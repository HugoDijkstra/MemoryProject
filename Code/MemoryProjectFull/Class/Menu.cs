using MemoryProjectFull;
using NewMemoryGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

class Menu : PanelBase{

    private Button b_login;

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

    private LoginPanel _panel;

    public Menu(int _width, int _height) : base(_width, _height)
    {

        _startGameCommand = new NetworkCommand("G:START", (x) => {
            this.Dispatcher.Invoke(() => { lobbyManager.startGame(); });
        }, false, true);

        // login pannel
        _panel = new LoginPanel(300, 500);
        _panel.setBackground("assets/images/ui_button.png");
        this.Center(UIPlacerMode.center, 0, _panel);
        _panel.rescale();

        // highscore panel
        //HighscorePanel highscorePanel = new HighscorePanel(300, 300);
        //this.Children.Add(highscorePanel);
        //UIPlacer.Center(UIPlacerMode.center, 0, highscorePanel);

        // player displays

        b_login = UIFactory.CreateButton("LOGIN", new Thickness(), new Point(70, 30), (x, y) =>{

            if (this.Children.Contains(_panel)){
                this.removeChild(_panel);
            }
            else
            {
                this.addChild(_panel);
            }

        });
        this.addChild(b_login);

        UIPlacer.CenterLeft(UIPlacerMode.top, 5, b_login);

        tb_nameInput = UIFactory.CreateTextBox(new Thickness(), new Point(200, 30), 20);
        this.addChild(tb_nameInput);

        b_name = UIFactory.CreateButton("Confirm Name", new Thickness(), new Point(200, 50), (x, y) =>
        {
            if (tb_nameInput.Text == string.Empty)
            {
                return;
            }

            tb_nameText.Text = tb_nameInput.Text;

            this.removeChild(tb_nameInput, b_name);
            this.addChild(tb_nameText, b_client, b_host);

            this.Center(UIPlacerMode.center, 3, tb_nameText, b_host, b_client);
        });

        this.addChild(b_name);
        this.Center(UIPlacerMode.center, 3, tb_nameInput, b_name);

        tb_nameText = UIFactory.CreateTextBlock("NULL", new Thickness(), new Point(200, 50), 20);

        b_host = UIFactory.CreateButton("HOST", new Thickness(), new Point(200, 50), (x, y) =>
        {
            start(true);
        });

        b_client = UIFactory.CreateButton("CLIENT", new Thickness(), new Point(200, 50), (x, y) =>
        {
            start(false);
        });

        b_start = UIFactory.CreateButton("START GAME", new Thickness(), new Point(200, 50), (x, y) =>
        {
            _startGameCommand.send("");
        });
    }

    public void start(bool _isHost)
    {
        this.removeChild(b_host);
        this.removeChild(b_client);

        if (_isHost)
        {
            NetworkManager.getInstance().create(NetworkType.Host, "127.0.0.1", 8001, (x) =>
            { // start host
                this.Dispatcher.Invoke(() => createLobby());
            });
        }
        else
        {
            NetworkManager.getInstance().create(NetworkType.Client, "192.168.2.52", 8001, (x) =>
            { // start client
                this.Dispatcher.Invoke(() => createLobby());
            });
        }

        NetworkHandler.getInstance().OnLostConnectionToHost += () =>
        { // on lost connection with host callback
            MessageBox.Show("Lost connection with host!");
        };


    }

    private void createLobby()
    {
        lobbyManager = new LobbyManager(tb_nameInput.Text);

        lobbyManager.OnStart += (x) =>
        {
            this.removeChild(b_start);

            gamepanel = new GamePanel(5, 4, 100, 200, "cats"); // init the game panel
            this.addChild(gamepanel);

            List<PlayerInfo> playerInfo = new List<PlayerInfo>();
            for (int i = 0; i < x.Length; i++){
                PlayerInfo panel = new PlayerInfo(x[i].name, new Point(300, 70), (PlayerInfo.ScreenLocation)i, 20);
                this.addChild(panel);
                playerInfo.Add(panel);
            }

            turnManager = new TurnManager(x, playerInfo, gamepanel);
        };

        if (NetworkHandler.getInstance().isHost())
        {
            this.addChild(b_start);
            this.Center(UIPlacerMode.center, 3, b_start);
        }
    }

}
