using System;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MemoryProjectFull
{
    class GamePanel : Grid
    {
        // for networking (create a network commmand)
        private NetworkCommand _OnFlip;
        private NetworkCommand _OnGridInit;

        // i need grid size x and y for this (other way to know wat card is wat, now i do it with x,y pos in array)
        private int gridSizeX;
        private int gridSizeY;

        bool firstRun;
        bool firstCardClicked;

        OnClickDoneArgs doneArgs;

        Card currentlyFlippingA, currentlyFlippingB;

        DispatcherTimer flipTimer;
        DispatcherTimer destroyAfterFlipTimer;
        Card waitForFlip;

        /// <summary>
        /// The cards on the field
        /// </summary>
        Card[,] cards;

        bool localPaused;

        int removeCardID;

        /// <summary>
        /// Constructor for the gamepanel that that creates the card and grid with set parameters
        /// </summary>
        /// <param name="widht">Amount of cards in the x axis</param>
        /// <param name="height">Amount of cards in the y axis</param>
        /// <param name="carSizeX">Size of the cards in the x axis</param>
        /// <param name="carSizeY">Size of the cards in the y axis</param>
        public GamePanel(int widht, int height, int carSizeX, int carSizeY, string theme)
        {
            flipTimer = new DispatcherTimer();
            destroyAfterFlipTimer = new DispatcherTimer();
            flipTimer.Start();
            destroyAfterFlipTimer.Start();

            removeCardID = 0;
            // for networking (i need the size of the grid for the for loop search, can be fixed with array x,y in card class)
            gridSizeX = widht;
            gridSizeY = height;

            // create netowork command
            _OnFlip = new NetworkCommand("G:CFLIP", // <-- network command id (G = 'global/game', S = 'server')
            (x) =>
            { // <-- callback when command is resieved
                this.Dispatcher.Invoke(() =>
                { // <-- do this when working with UI (stops calling objects from networking thread)
                    int xPos = int.Parse(x[0]);
                    int yPos = int.Parse(x[1]);

                    // normaal shit
                    CardClicked(cards[xPos, yPos]);
                });
            },
            false, // <-- only accept command with divrent ID then this client (true/false)
            true); // <-- auto activate command (add command to command listener list)

            _OnGridInit = new NetworkCommand("G:CGRID", (x) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    Card.callback = HandleCallback;
                    childerenCount = 100;
                    Run(x, carSizeX, carSizeY);
                });

            },
            false,
            true);

            initRun(widht, height, carSizeX, carSizeY, theme);
        }

        ~GamePanel()
        {
            Card.callback = null;
        }

        /// <summary>
        /// Handle on click callback
        /// </summary>
        /// <param name="c"></param>
        private void HandleCallback(Card c)
        {

            if (localPaused)
                return;

            // send the command (this is to much work, maby store grid x, y in card class)
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (cards[x, y] == c)
                    {
                        _OnFlip.send(new string[2] { x.ToString(), y.ToString() });
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Build the actual game grid
        /// </summary>
        /// <param name="cards">The cards for the game</param>
        /// <param name="xAmount">amount of cards in the widht</param>
        /// <param name="yAmount">amount of cards in height</param>
        /// <param name="cardSizeX">size of the card</param>
        /// <param name="cardSizeY">size of the card</param>
        /// <param name="cardMargins">space between the cards</param>
        public void Build(Card[,] cards, int xAmount, int yAmount, int cardSizeX, int cardSizeY, int cardMargins)
        {
            //Cards.Count() needs to be more or equale too (x * y)
            if (cards.Length < xAmount * yAmount)
            {
                Console.WriteLine("Cards.Count() needs to be more or equale too (x * y)");
                return;
            }

            GridLength gridheight = new GridLength(cardSizeY + cardMargins * 2);
            GridLength gridWidth = new GridLength(cardSizeX + cardMargins * 2);

            Thickness margins = new Thickness();

            margins.Left = (MainWindow.SCREEN_WIDTH / 2) - ((cardSizeX + cardMargins * 2) * (float)xAmount / 2f);
            margins.Top = (MainWindow.SCREEN_HEIGHT / 2) - ((cardSizeY + cardMargins * 2) * (float)yAmount / 2f);

            this.Margin = margins;
            for (int i = 0; i < xAmount; i++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = gridWidth });
                for (int j = 0; j < yAmount; j++)
                {
                    this.RowDefinitions.Add(new RowDefinition() { Height = gridheight });
                    Border b = new Border();
                    Card c = cards[i, j];
                    b.Height = c.Height;
                    b.Width = c.Width;
                    b.Margin = new Thickness(cardMargins);
                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, j);
                    b.Child = c;
                    Children.Add(b);
                }
            }
            doneArgs = new OnClickDoneArgs();
            firstCardClicked = false;
            localPaused = false;
        }

        /// <summary>
        /// Activate the grid so the player can use it
        /// </summary>
        public void Activate()
        {
            localPaused = false;
        }

        /// <summary>
        /// deactivate the grid so the player can't use it
        /// </summary>
        public void Deactivate()
        {
            localPaused = true;
        }


        ///TODO documentation
        /// <summary>
        /// handle the card being clicked
        /// </summary>
        /// <param name="c"></param>
        private void CardClicked(Card c)
        {
            if (waitForFlip != null)
                if (waitForFlip.IsFlipping())
                    return;
            if (doneArgs.firstCard == null)
            {
                doneArgs.firstCard = c;
                c.Flip();
                firstCardClicked = true;
            }
            else
            {
                doneArgs.secondCard = c;
                c.Flip();
                doneArgs.Correct = (doneArgs.firstCard.ID == c.ID);
                if (!doneArgs.Correct)
                {
                    currentlyFlippingA = doneArgs.firstCard;
                    currentlyFlippingB = doneArgs.secondCard;

                    waitForFlip = currentlyFlippingB;
                    flipTimer.Tick += FlipTimer_Tick;
                }
                waitForFlip = currentlyFlippingB;
                OnClickDone(doneArgs);
                firstCardClicked = false;
                doneArgs = new OnClickDoneArgs();
            }
        }

        /// <summary>
        /// Wait until the card is flipped, then flip back
        /// </summary>
        private void FlipTimer_Tick(object sender, EventArgs e)
        {
            if (!waitForFlip.IsFlipping())
            {
                currentlyFlippingA.Flip();
                currentlyFlippingB.Flip();
                flipTimer.Tick -= FlipTimer_Tick;
            }
        }

        /// <summary>
        /// Handles the removal of cards
        /// </summary>
        /// <param name="id"></param>
        public void RemoveCard(int id)
        {
            removeCardID = id;
            firstRun = true;
            this.Dispatcher.Invoke(() => { destroyAfterFlipTimer.Tick += DestroyTimer_Tick; });
        }

        /// <summary>
        /// Destroys the cards when they are done flipping
        /// </summary>
        public void DestroyTimer_Tick(object sender, EventArgs e)
        {
            if (firstRun)
            {
                firstRun = false;
                return;
            }

            if (!waitForFlip.IsFlipping())
            {
                for (int x = 0; x < gridSizeX; x++)
                    for (int y = 0; y < gridSizeY; y++)
                        if (cards[x, y].ID == removeCardID)
                            Children.Remove((Border)(cards[x, y].Parent));
                destroyAfterFlipTimer.Tick -= DestroyTimer_Tick;
            }
        }

        int childerenCount = 0;
        public bool IsGridEmpty()
        {
            return childerenCount <= 0;
        }

        /// <summary>
        /// On click handler
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClickDone(OnClickDoneArgs e)
        {
            EventHandler<OnClickDoneArgs> eventHandler = onClickDone;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Onclick args
        /// </summary>
        public class OnClickDoneArgs : EventArgs
        {
            public Card firstCard;
            public Card secondCard;
            public bool Correct;
        }

        /// <summary>
        /// Onclickdone eventhandler
        /// </summary>
        public event EventHandler<OnClickDoneArgs> onClickDone;

        /// <summary>
        /// handles the starting of the game
        /// </summary>
        /// <param name="x">amount of cards horizontal</param>
        /// <param name="y">amount of cards vertical</param>
        /// <param name="cardSizeX">size of the cards horizontal</param>
        /// <param name="cardSizeY">size of the cards vertical</param>
        /// <param name="theme">The chosen theme for the game</param>
        public void initRun(int x, int y, int cardSizeX, int cardSizeY, string theme)
        {
            if (NetworkHandler.getInstance().isHost())
            {
                List<string> list = ImageGetter.GetUrlsByTheme(theme, (x * y) / 2); // <-- list of url's to images

                string[] message = new string[x * y + 2];

                message[0] = x.ToString();
                message[1] = y.ToString();

                string[] cardData = new string[x * y];

                int listCount = 0;
                for (int i = 0; i < cardData.Length; i += 2)
                {
                    string data = list[listCount] + ";" + i.ToString(); // <-- add url and id
                    cardData[i] = data;
                    cardData[i + 1] = data;

                    listCount++;
                }

                ExtensionMethods.Shuffle<string>(cardData);

                for (int i = 2; i < message.Length; i++)
                {
                    message[i] = cardData[i - 2];
                }

                //       Run(message, cardSizeX, cardSizeY);
                _OnGridInit.send(message);
            }
        }

        /// <summary>
        /// Handle the prebuilding of the deck
        /// </summary>
        /// <param name="data">The urls for the loading of images</param>
        /// <param name="cardSizeX">size of the cards horizontal</param>
        /// <param name="cardSizeY">size of the cards vertical</param>
        public void Run(string[] data, int cardSizeX, int cardSizeY)
        {

            int sizeX = int.Parse(data[0]);
            int sizeY = int.Parse(data[1]);

            cards = new Card[sizeX, sizeY];

            // init back card

            Card.BackImage = new BitmapImage(new Uri("assets/images/background_card.png", UriKind.RelativeOrAbsolute));

            int dataCount = 2;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    // unpack data
                    string[] dataPack = data[dataCount].Split(';');
                    // get url and id
                    string imageUrl = dataPack[0];
                    int id = int.Parse(dataPack[1]);
                    // create image
                    BitmapImage image = ImageGetter.GetImageFromWeb(imageUrl, cardSizeX);

                    // create card
                    cards[x, y] = new Card(id, new Size(cardSizeX, cardSizeY), new Point(0, 0), image);

                    // next data pack
                    dataCount++;
                }
            }

            Build(cards, sizeX, sizeY, cardSizeX, cardSizeY, 10);
        }
    }
    public static class ExtensionMethods
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random r = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                int rand = r.Next(0, list.Count);
                T oldValue = list[rand];
                list[i] = oldValue;
                list[rand] = t;
            }
        }
    }
}
