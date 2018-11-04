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

namespace MemoryProjectFull
{
    public class EscapeMenu : Grid
    {

        private static readonly Uri BACKGROUND_IMAGE_PATH = new Uri("assets/images/background_panel.png", UriKind.RelativeOrAbsolute);

        private const int CONTENT_ROWS = 2;
        private const int CONTENT_COLS = 2;

        #region ESCAPEMENU_SETUP

        private static readonly Size UNIFORM_BUTTON_SIZE = new Size(double.NaN, double.NaN);

        private void SetupHeaderText()
        {
            headerText = UIFactory.CreateTextBlock("Game Menu", new Thickness(16, 16, 16, 8), new Size(double.NaN, double.NaN), 16); //TODO: ADD TEXT.

            headerText.HorizontalAlignment = HorizontalAlignment.Center;
            headerText.VerticalAlignment   = VerticalAlignment.Center;

            Grid.SetRow(headerText, 0);
            Grid.SetColumnSpan(headerText, CONTENT_COLS);

            (this.Children).Add(headerText);
        }

        private void SetupButtons()
        {
            backButton  = UIFactory.CreateButton("Back", new Thickness(16, 8, 16, 16), UNIFORM_BUTTON_SIZE, null); //TODO: SIDNEY'S CALLBACK HERE.

            backButton.Padding = new Thickness(16, 4, 16, 4);

            backButton.HorizontalAlignment = HorizontalAlignment.Center;
            backButton.VerticalAlignment   = VerticalAlignment.Center;

            backButton.HorizontalContentAlignment = HorizontalAlignment.Center;
            backButton.VerticalContentAlignment   = VerticalAlignment.Center;

            Grid.SetRow(backButton, 1);
            Grid.SetColumn(backButton, 0);

            (this.Children).Add(backButton);

            resetButton = UIFactory.CreateButton("Reset", new Thickness(16, 8, 16, 16), UNIFORM_BUTTON_SIZE, null); //TODO: SIDNEY'S CALLBACK HERE.

            resetButton.Padding = new Thickness(16, 4, 16, 4);

            resetButton.HorizontalAlignment = HorizontalAlignment.Center;
            resetButton.VerticalAlignment   = VerticalAlignment.Center;

            resetButton.HorizontalContentAlignment = HorizontalAlignment.Center;
            resetButton.VerticalContentAlignment   = VerticalAlignment.Center;

            Grid.SetRow(resetButton, 1);
            Grid.SetColumn(resetButton, 1);

            (this.Children).Add(resetButton);
        }

        #endregion

        public EscapeMenu(bool startHidden)
        {
            this.Background = new ImageBrush(new BitmapImage(BACKGROUND_IMAGE_PATH));

            this.Margin = new Thickness(0, 0, 0, 0);

            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment   = VerticalAlignment.Center;

            var rows = this.RowDefinitions;
            for (int y = 0; y < CONTENT_ROWS; y++) rows.Add(new RowDefinition());

            var cols = this.ColumnDefinitions;
            for (int x = 0; x < CONTENT_COLS; x++) cols.Add(new ColumnDefinition());

            SetupHeaderText();
            SetupButtons();

            if (startHidden) this.Hide();
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public bool IsShown
        {
            get { return (this.Visibility == Visibility.Visible); }
        }

        private TextBlock headerText;
        private Button backButton, resetButton;

    }
}
