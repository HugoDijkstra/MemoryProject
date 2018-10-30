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

            //this.FontFamily = new FontFamily(new Uri("assets/fonts/font.ttf", UriKind.RelativeOrAbsolute), "game_font");

            // add autio
            AudioManager.RegisterAudio("music_menu", "assets/audio/button_click.mp3");
            AudioManager.RegisterAudio("music_game", "assets/audio/button_click.mp3");

            AudioManager.RegisterAudio("button_click", "assets/audio/button_click.mp3");
            AudioManager.RegisterAudio("whoosh_1", "assets/audio/whoosh_1.mp3");
            AudioManager.RegisterAudio("whoosh_2", "assets/audio/whoosh_2.mp3");
            AudioManager.RegisterAudio("notification_1", "assets/audio/notification_1.mp3");
            AudioManager.RegisterAudio("notification_2", "assets/audio/notification_2.mp3");

            AudioManager.RegisterAudio("card_done", "assets/audio/card_done.mp3");
            AudioManager.RegisterAudio("card_fail", "assets/audio/card_fail.mp3");

            var horizontalBorderHeight = SystemParameters.ResizeFrameHorizontalBorderHeight;
            var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
            var captionHeight = SystemParameters.CaptionHeight;

            SCREEN_HEIGHT = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            SCREEN_WIDTH = (int)System.Windows.SystemParameters.PrimaryScreenWidth;

            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;

            Width = SCREEN_WIDTH + 2 * verticalBorderWidth;
            Height = SCREEN_HEIGHT + captionHeight + 2 * horizontalBorderHeight;

            MemoryDatabase.init();

            Account.Load();

            if (!MemoryDatabase.database.CheckTableExistence("users")) { 
                SortedList<string, DatabaseReader.MySqlDataType> paramList = new SortedList<string, DatabaseReader.MySqlDataType>();
                paramList.Add("id", DatabaseReader.MySqlDataType.Float);
                paramList.Add("name", DatabaseReader.MySqlDataType.Text);
                paramList.Add("password", DatabaseReader.MySqlDataType.Text);
                paramList.Add("wins", DatabaseReader.MySqlDataType.Float);
                paramList.Add("loses", DatabaseReader.MySqlDataType.Float);
                MemoryDatabase.database.CreateTable("users", paramList);
            }

            //SortedList<string, string> tableData = new SortedList<string, string>();
            //tableData.Add("id", "0");
            //tableData.Add("name", "jan");
            //tableData.Add("password", "pa22word");
            //tableData.Add("wins", "0");
            //tableData.Add("loses", "0");
            //MemoryDatabase.database.AddDataToTable("users", tableData);

            this.AddChild(new Menu(SCREEN_WIDTH, SCREEN_HEIGHT));
        }
    }
}
