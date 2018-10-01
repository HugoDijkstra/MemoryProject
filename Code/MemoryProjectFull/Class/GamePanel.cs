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
    public partial class Card : Control { public Image image; }
    class GamePanel : Grid
    {
        public GamePanel()
        {
            Run();
        }

        public GamePanel(int widht, int height)
        {
            Run();
        }

        bool firstCardClicked;

        OnClickDoneArgs doneArgs;

        public void Build(List<Card> cards, int xAmount, int yAmount)
        {
            //Cards.Count() needs to be more or equale too (x * y)
            if (cards.Count() < xAmount * yAmount)
            {
                Console.WriteLine("Cards.Count() needs to be more or equale too (x * y)");
                return;
            }

            Width = 400 * xAmount;
            Height = 400 * yAmount;

            int k = 0;
            for (int i = 0; i < xAmount; i++)
            {
                this.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < yAmount; j++)
                {
                    this.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    k++;
                    Card c = cards[k];
                    Grid.SetColumn(c.image, j);
                    Grid.SetRow(c.image, i);
                    Children.Add(c.image);
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

        public void Run()
        {
            List<Card> cards = new List<Card>();
            List<BitmapImage> bitmapImages = ImageGetter.GetImagesByTheme("Dank memes", 30, 400);
            for (int i = 0; i < bitmapImages.Count; i++)
            {
                Image im = new Image();
                im.BeginInit();
                im.Source = bitmapImages[i];
                im.MaxHeight = 400;
                im.MaxHeight = 400;
                im.Stretch = Stretch.Fill;
                im.Width = 200;
                im.EndInit();
                Card card = new Card() { image = im };
                cards.Add(card);
            }
            Build(cards, 8, 8);
        }
    }
    partial class Card
    {

    }
}
