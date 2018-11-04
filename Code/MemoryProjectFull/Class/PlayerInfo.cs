using MemoryProjectFull.Class;
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
    /// <summary>
    /// Player info panel
    /// </summary>
    public class PlayerInfo : Canvas
    {
        public string playerName { get; private set; }
        public int cards;

        private TextBlock block;

        ImageBrush inactive;
        ImageBrush active;

        public enum ScreenLocation
        {
            TopRight,
            TopLeft,
            BottomRight,
            BottomLeft
        }

        ScreenLocation screenLocation;

        public Point size;

        /// <summary>
        /// Set all atributes
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="size"></param>
        /// <param name="location"></param>
        /// <param name="fontSize"></param>
        public PlayerInfo(string playerName, Point size, ScreenLocation location, int fontSize)
        {
            this.screenLocation = location;
            this.size = size;
            this.playerName = playerName;
            this.Width = size.X;
            this.Height = size.Y;
            this.cards = 0;

            block = new TextBlock();

            block.Width = size.X - 20;
            block.Height = size.Y - 20;
            block.Margin = new Thickness(10);



            this.Children.Add(block);

            UpdateBox();

            block.FontSize = fontSize;

            inactive = new ImageBrush() { ImageSource = new BitmapImage((new Uri("assets/images/background_panel.png", UriKind.RelativeOrAbsolute))) };
            active = new ImageBrush() { ImageSource = new BitmapImage((new Uri("assets/images/background_panel_active.png", UriKind.RelativeOrAbsolute))) };
            this.Background = inactive;
            UpdateScreenLocation();
        }

        /// <summary>
        /// Set color using a color object
        /// </summary>
        /// <param name="_color"></param>
        public void setBackgroundColor(SolidColorBrush _color)
        {
            this.Background = _color;
        }

        /// <summary>
        /// Set the name of the player represented by this board
        /// </summary>
        /// <param name="playerName"></param>
        public void SetName(string playerName)
        {
            this.playerName = playerName;
            UpdateBox();
        }

        /// <summary>
        /// Set points in cards collected
        /// </summary>
        /// <param name="cardAmount"></param>
        public void SetCards(int cardAmount)
        {
            this.Dispatcher.Invoke(() => {
                cards = cardAmount;
                UpdateBox();
            });
        }

        /// <summary>
        /// Tell the board if its the active players turn
        /// </summary>
        /// <param name="turn"></param>
        public void SetTurn(bool turn)
        {
            this.Dispatcher.Invoke(() =>
            {
                Background = turn ? active : inactive;
            });
        }

        /// <summary>
        /// Update the box content
        /// </summary>
        private void UpdateBox()
        {
            block.Text = "Name: " + playerName + "\nCards: " + cards;
        }

        /// <summary>
        /// update the location on the screen
        /// </summary>
        private void UpdateScreenLocation()
        {
            Thickness location = new Thickness();
            switch (screenLocation)
            {
                case ScreenLocation.TopLeft:
                    location.Left = 10;
                    location.Top = 10;
                    block.TextAlignment = TextAlignment.Left;
                    break;
                case ScreenLocation.TopRight:
                    location.Left = MainWindow.SCREEN_WIDTH - (Width + 10);
                    location.Top = 10;
                    block.TextAlignment = TextAlignment.Right;
                    break;
                case ScreenLocation.BottomLeft:
                    location.Left = 10;
                    location.Top = MainWindow.SCREEN_HEIGHT - (Height + 10);
                    block.TextAlignment = TextAlignment.Left;
                    break;
                case ScreenLocation.BottomRight:
                    location.Left = MainWindow.SCREEN_WIDTH - (Width + 10);
                    location.Top = MainWindow.SCREEN_HEIGHT - (Height + 10);
                    block.TextAlignment = TextAlignment.Right;
                    break;
            }
            this.Margin = location;
        }

    }
}
