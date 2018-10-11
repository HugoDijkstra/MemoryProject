using MemoryProjectFull;
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
        public MainWindow()
        {
            InitializeComponent();

            const int snugContentWidth = 1300;
            const int snugContentHeight = 700;

            var horizontalBorderHeight = SystemParameters.ResizeFrameHorizontalBorderHeight;
            var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
            var captionHeight = SystemParameters.CaptionHeight;

            Width = snugContentWidth + 2 * verticalBorderWidth;
            Height = snugContentHeight + captionHeight + 2 * horizontalBorderHeight;

            this.AddChild(new Menu());
        }
    }
}

class Menu : Canvas {

    private Button b_host;
    private Button b_client;

    public Menu() : base() {

        // host button
        b_host = new Button();
        b_host.Content = "HOST";
        b_host.Margin = new Thickness(100, 100, 0, 0);
        b_host.Width = 50;
        b_host.Height = 50;
        b_host.Click += (x, y) => {
            start(true);
        };
        this.Children.Add(b_host);

        // client button
        b_client = new Button();
        b_client.Content = "CLIENT";
        b_client.Margin = new Thickness(100, 150, 0, 0);
        b_client.Width = 50;
        b_client.Height = 50;
        b_client.Click += (x, y) => {
            start(false);
        };
        this.Children.Add(b_client);
    }

    public void start(bool _isHost) {
        this.Children.Remove(b_host);
        this.Children.Remove(b_client);

        if (_isHost) {
            NetworkManager.getInstance().create(NetworkType.Host, "127.0.0.1", 8001, (x) => { // start host
                MessageBox.Show(string.Format("host -> id: {0}", x));
            });
        } else {
            NetworkManager.getInstance().create(NetworkType.Client, "127.0.0.1", 8001, (x) => { // start client
                MessageBox.Show(string.Format("client -> id: {0}", x));
            });
        }

        NetworkHandler.getInstance().OnLostConnectionToHost += () => { // on lost connection with host callback
            MessageBox.Show("Lost connection with host!");
        };

        //LobbyManager lobbyManager = new LobbyManager();

        //lobbyManager.OnStart += () => {
        //    Console.WriteLine("Game Has Started");
        //};

        // at the end
        GamePanel gamePanel = new GamePanel(5, 4, 200, 1000, "Cats"); // init the game panel
        this.Children.Add(gamePanel);
    }

}
