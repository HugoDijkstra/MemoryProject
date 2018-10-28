using MemoryProjectFull;
using NewMemoryGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemoryProjectFull
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int SCREEN_WIDTH = 1300;
        public static int SCREEN_HEIGHT = 900;

        public MainWindow()
        {
            InitializeComponent();

            var horizontalBorderHeight = SystemParameters.ResizeFrameHorizontalBorderHeight;
            var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
            var captionHeight = SystemParameters.CaptionHeight;

           Height = SCREEN_HEIGHT;
           Width = SCREEN_WIDTH;

            this.ResizeMode = ResizeMode.NoResize;

            Width = SCREEN_WIDTH + 2 * verticalBorderWidth;
            Height = SCREEN_HEIGHT + captionHeight + 2 * horizontalBorderHeight;

            this.AddChild(new Menu());
        }
    }
}
