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
        public MainWindow()
        {
            ImageGetter.GetUrlsByTheme("Dog", 100);
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

    private TextBox tb_nameInput;
    private Button b_name;

    private TextBlock tb_nameText;
    private Button b_host;
    private Button b_client;

    private Button b_start;

    private LobbyManager lobbyManager;
    private GamePanel gamepanel;
    private TurnManager turnManager;

    private NetworkCommand _startGameCommand;

    public Menu() : base() {

        _startGameCommand = new NetworkCommand("G:START", (x) => {
            this.Dispatcher.Invoke(() => { lobbyManager.startGame(); });
        }, false, true);

        tb_nameInput = UIFactory.CreateTextBox(new Thickness(), new Point(200, 30), 20);
        this.Children.Add(tb_nameInput);

        b_name = UIFactory.CreateButton("Confirm Name", new Thickness(), new Point(200, 50), (x, y) => {
            if (tb_nameInput.Text == string.Empty) {
                return;
            }

            tb_nameText.Text = tb_nameInput.Text;

            this.Children.Remove(tb_nameInput);
            this.Children.Remove(b_name);

            this.Children.Add(tb_nameText);
            this.Children.Add(b_client);
            this.Children.Add(b_host);

            UIFactory.Center(1300, 300, 3, tb_nameText, b_host, b_client);
        });

        this.Children.Add(b_name);
        UIFactory.Center(1300, 300, 3, tb_nameInput, b_name);

        tb_nameText = UIFactory.CreateTextBlock("NULL", new Thickness(), new Point(200, 50), 20);

        b_host = UIFactory.CreateButton("HOST", new Thickness(), new Point(200, 50), (x, y) => {
            start(true);
        });
        
        b_client = UIFactory.CreateButton("CLIENT", new Thickness(), new Point(200, 50), (x, y) => {
            start(false);
        });

        b_start = UIFactory.CreateButton("START GAME", new Thickness(), new Point(200, 50), (x, y) => {
            _startGameCommand.send("");
        });

    }

    public void start(bool _isHost) {
        this.Children.Remove(b_host);
        this.Children.Remove(b_client);

        if (_isHost) {
            NetworkManager.getInstance().create(NetworkType.Host, "127.0.0.1", 8001, (x) => { // start host
                this.Dispatcher.Invoke(() => createLobby());
            });
        } else {
            NetworkManager.getInstance().create(NetworkType.Client, "127.0.0.1", 8001, (x) => { // start client
                this.Dispatcher.Invoke(() => createLobby());
            });
        }

        NetworkHandler.getInstance().OnLostConnectionToHost += () => { // on lost connection with host callback
            MessageBox.Show("Lost connection with host!");
        };

        
    }

    private void createLobby() {
        lobbyManager = new LobbyManager(tb_nameInput.Text);

        lobbyManager.OnStart += (x) => {
            this.Children.Remove(b_start);

            gamepanel = new GamePanel(5, 4, 200, 1000, "cats"); // init the game panel
            this.Children.Add(gamepanel);

            turnManager = new TurnManager(x, gamepanel);
        };

        if (NetworkHandler.getInstance().isHost()) {
            this.Children.Add(b_start);
            UIFactory.Center(1300, 300, 3, b_start);
        }
    }

}
