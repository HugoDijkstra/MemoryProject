using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MemoryProjectFull
{
    public class PlayerInfo : Canvas
    {
        public string playerName { get; private set; }
        public int cards;

        private TextBlock block;

        public enum ScreenLocation
        {
            TopRight,
            TopLeft,
            BottomRight,
            BottomLeft
        }

        ScreenLocation screenLocation;

        public Point size;

        public PlayerInfo(string playerName, Point size, ScreenLocation location, int fontSize)
        {
            this.screenLocation = location;
            this.size = size;
            this.playerName = playerName;
            this.Width = size.X;
            this.Height = size.Y;
            this.cards = 0;

            block = new TextBlock();
            this.Children.Add(block);

            UpdateBox();

            block.FontSize = fontSize;

            this.Background = new ImageBrush() { ImageSource = new BitmapImage((new Uri("assets/images/background_panel.png", UriKind.RelativeOrAbsolute))) };

            UpdateScreenLocation();
        }

        public void setBackground(SolidColorBrush _color)
        {
            this.Background = _color;
        }

        public void SetName(string playerName)
        {
            this.playerName = playerName;
            UpdateBox();
        }

        public void SetCards(int cardAmount)
        {
            cards = cardAmount;
            UpdateBox();
        }

        private void UpdateBox()
        {
            block.Text = "Name: " + playerName + "\nCards: " + cards;
        }

        private void UpdateScreenLocation()
        {
            Thickness location = new Thickness();
            switch (screenLocation)
            {
                case ScreenLocation.TopLeft:
                    location.Left = -10;
                    location.Top = -10;
                    block.TextAlignment = TextAlignment.Left;
                    break;
                case ScreenLocation.TopRight:
                    location.Left = MainWindow.SCREEN_WIDTH - (Width - 10);
                    location.Top = -10;
                    block.TextAlignment = TextAlignment.Right;
                    break;
                case ScreenLocation.BottomLeft:
                    location.Left = -10;
                    location.Top = MainWindow.SCREEN_HEIGHT - (Height - 10);
                    block.TextAlignment = TextAlignment.Left;
                    break;
                case ScreenLocation.BottomRight:
                    location.Left = MainWindow.SCREEN_WIDTH - (Width - 10);
                    location.Top = MainWindow.SCREEN_HEIGHT - (Height - 10);
                    block.TextAlignment = TextAlignment.Right;
                    break;
            }
            this.Margin = location;
        }

    }
}
