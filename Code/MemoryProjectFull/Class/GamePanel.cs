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

namespace MemoryProjectFull
{
    class GamePanel : Grid
    {

        bool firstCardClicked;

        OnClickDoneArgs doneArgs;

        Card[,] cards;
        public GamePanel()
        {
            Run(4, 4);
        }

        /// <summary>
        /// GamePanel Constructor FOR DEBUGGING PURPOSE
        /// </summary>
        /// <param name="widht"></param>
        /// <param name="height"></param>
        public GamePanel(int widht, int height)
        {
            Run(widht, height);
        }

        /// <summary>
        /// Constructor for the gamepanel that that creates the card and grid with set parameters
        /// </summary>
        /// <param name="widht">Amount of cards in the x axis</param>
        /// <param name="height">Amount of cards in the y axis</param>
        /// <param name="carSizeX">Size of the cards in the x axis</param>
        /// <param name="carSizeY">Size of the cards in the y axis</param>
        public GamePanel(int widht, int height, int carSizeX, int carSizeY)
        {
            Run(widht, height, carSizeX, carSizeY);
        }

        public void Build(Card[,] cards, int xAmount, int yAmount)
        {
            //Cards.Count() needs to be more or equale too (x * y)
            if (cards.Length < xAmount * yAmount)
            {
                Console.WriteLine("Cards.Count() needs to be more or equale too (x * y)");
                return;
            }

            Width = 200 * xAmount;
            Height = 250 * yAmount;

            VerticalAlignment = VerticalAlignment.Center;
            for (int i = 0; i < xAmount; i++)
            {
                this.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < yAmount; j++)
                {
                    this.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    Card c = cards[i, j];
                    c.Margin = new Thickness(10);
                    Border border = new Border();
                    border.Child = c;
                    Grid.SetColumn(((Card)(border.Child)), i);
                    Grid.SetRow(((Card)(border.Child)), j);
                    Children.Add(((Card)(border.Child)));
                }
            }
            doneArgs = new OnClickDoneArgs();
            firstCardClicked = false;
        }

        public void Build(Card[,] cards, int xAmount, int yAmount, int carSizeX, int carSizeY)
        {
            //Cards.Count() needs to be more or equale too (x * y)
            if (cards.Length < xAmount * yAmount)
            {
                Console.WriteLine("Cards.Count() needs to be more or equale too (x * y)");
                return;
            }

            Width = (carSizeX + 10) * xAmount;
            Height = (carSizeY + 10) * yAmount;

            for (int i = 0; i < xAmount; i++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                for (int j = 0; j < yAmount; j++)
                {
                    this.RowDefinitions.Add(new RowDefinition());
                    Card c = cards[i, j];
                    c.Margin = new Thickness(10);
                    Grid.SetColumn(c, i);
                    Grid.SetRow(c, j);
                    Children.Add(c);
                }
            }
            doneArgs = new OnClickDoneArgs();
            firstCardClicked = false;
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        private void CardClicked(Card c)
        {
            if (!firstCardClicked)
            {
                doneArgs.firstCard = c;
            }
            else
            {
                doneArgs.secondCard = c;
                doneArgs.Correct = (doneArgs.firstCard == c);

                OnClickDone(doneArgs);

            }
        }

        protected virtual void OnClickDone(OnClickDoneArgs e)
        {
            EventHandler<OnClickDoneArgs> eventHandler = onClickDone;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public class OnClickDoneArgs : EventArgs
        {
            public Card firstCard;
            public Card secondCard;
            public bool Correct;
        }

        public event EventHandler<OnClickDoneArgs> onClickDone;

        public void Run(int x, int y)
        {
            cards = new Card[x, y];
            List<BitmapImage> bitmapImages = ImageGetter.GetImagesByTheme("Dank memes", x * y, 200);
            int image = 0;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Image im = new Image();
                    Card card = new Card(image, new Size(200, 300), new Point(0, 0), bitmapImages[image]);
                    cards[i, j] = card;
                }
            }
            Build(cards, x, y);
        }
        public void Run(int x, int y, int cardSizeX, int cardSizeY)
        {
            cards = new Card[x, y];
            List<BitmapImage> bitmapImages = ImageGetter.GetImagesByTheme("Dank memes", x * y, cardSizeY);
            int image = 0;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Card card = new Card(image, new Size(cardSizeX, cardSizeY), new Point(0, 0), bitmapImages[image]);
                    cards[i, j] = card;
                }
            }
            Build(cards, x, y, cardSizeX, cardSizeY);
        }
    }
}
