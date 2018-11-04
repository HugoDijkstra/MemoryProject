using NewMemoryGame;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Size = System.Windows.Point;

namespace MemoryProjectFull.Class
{
    class EndGame : Grid
    {
        private static readonly Uri BACKGROUND_IMAGE_PATH = new Uri("assets/images/score_screen.png", UriKind.RelativeOrAbsolute);

        private TextBlock headerText;
        private TextBlock firstText;
        private TextBlock secondText;
        private TextBlock thirdText;

        private Button menuButton;

        private static readonly Size UNIFORM_BUTTON_SIZE = new Size(double.NaN, double.NaN);

        private const int CONTENT_ROWS = 2;
        private const int CONTENT_COLS = 2;

        public EndGame()
        {
            this.Background = new ImageBrush(new BitmapImage(BACKGROUND_IMAGE_PATH));
            this.Margin = new Thickness(0, 0, 0, 0);

            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;

            this.Width = 1185;
            this.Height = 658;

            SetupHeaderText();
            SetupButtons();
        }

        private void SetupHeaderText()
        {
            headerText = UIFactory.CreateTextBlock("Score Screen", new Thickness(16, 16, 16, 8), new Size(double.NaN, double.NaN), 16);

            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            headerText.VerticalAlignment = VerticalAlignment.Top;
            headerText.Margin = new Thickness(0, 60, 0, 0);
            headerText.FontSize = 40;

            Grid.SetRow(headerText, 0);
            Grid.SetColumnSpan(headerText, CONTENT_COLS);

            (this.Children).Add(headerText);
        }

        private void SetPlayerInfoText()
        {
            firstText = UIFactory.CreateTextBlock("", new Thickness(16, 16, 16, 8), new Size(double.NaN, double.NaN), 16);
            secondText = UIFactory.CreateTextBlock("", new Thickness(16, 16, 16, 8), new Size(double.NaN, double.NaN), 16);
            thirdText = UIFactory.CreateTextBlock("", new Thickness(16, 16, 16, 8), new Size(double.NaN, double.NaN), 16);

            firstText.HorizontalAlignment = HorizontalAlignment.Center;
            firstText.VerticalAlignment = VerticalAlignment.Top;
            firstText.Margin = new Thickness(0, 60, 0, 0);
            firstText.FontSize = 40;

            //Grid.SetRow(headerText, 0);
            //Grid.SetColumnSpan(headerText, CONTENT_COLS);

            (this.Children).Add(firstText);
        }

        private void SetupButtons()
        {
            menuButton = UIFactory.CreateButton("Menu", new Thickness(16, 8, 16, 8), UNIFORM_BUTTON_SIZE, null);

            menuButton.Padding = new Thickness(28, 16, 28, 16);
            menuButton.Margin = new Thickness(0, 0, 0, 50);

            menuButton.HorizontalAlignment = HorizontalAlignment.Center;
            menuButton.VerticalAlignment = VerticalAlignment.Bottom;

            Grid.SetRow(menuButton, 1);
            Grid.SetColumn(menuButton, 1);

            (this.Children).Add(menuButton);
        }
    }
}
