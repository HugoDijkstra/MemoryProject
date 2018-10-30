using MemoryProjectFull;
using NewMemoryGame;
using NotificationsWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

class Menu : PanelBase{

    private Button b_login;
    private Button b_highscore;

    // main menu
    private TextBlock tb_namemessage;
    private TextBox tb_ip;
    private Button b_client;
    private Button b_host;
    private Button b_quit;

    // lobby menu
    private TextBlock tb_lobbyname;
    private TextBlock tb_lobbyscore;
    private TextBlock tb_lobbygenredisplay;
    private TextBox tb_lobbygenre;
    private Button b_loggyconfirmgenre;
    private Button b_lobbyback;
    private Button b_lobbystartgame;
    private TextBlock tb_lobbyplayerdisplay;

    // loading screen
    private TextBlock tb_loadingmessage;
    private Button b_loadingback;

    private LobbyManager lobbyManager;
    private GamePanel gamepanel;
    private TurnManager turnManager;

    private NetworkCommand _startGameCommand;
    private NetworkCommand _onGengreChange;

    private LoginPanel _loginPanel;
    private HighscorePanel _highscorePanel;
    private PanelBase _backgroundGame;

    private NotificationHandler _notification;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="_width">panel width</param>
    /// <param name="_height">panel height</param>
    public Menu(int _width, int _height) : base(_width, _height){
        this.setBackground("assets/images/background_game.jpg");

        _startGameCommand = new NetworkCommand("G:START", (x) => {
            this.Dispatcher.Invoke(() => { initNone(); lobbyManager.startGame(); });
        }, false, true);

        _onGengreChange = new NetworkCommand("G:GENCHANGE", (x) => {
            this.Dispatcher.Invoke(() => { tb_lobbygenredisplay.Text = x[0]; });
        });

        // game background
        _backgroundGame = new PanelBase(MainWindow.SCREEN_WIDTH, MainWindow.SCREEN_HEIGHT);
        _backgroundGame.setBackground("assets/images/background_menu.png");
        this.Center(UIPlacerMode.center, 0, _backgroundGame);

        // notifications
        _notification = new NotificationHandler();
        this.addChild(_notification);

        NotificationManager.OnRequestNotification("gang gang nibbah");

        // login pannel
        _loginPanel = new LoginPanel(300, 500);
        _loginPanel.setBackground("assets/images/background_panel.png");
        this.Center(UIPlacerMode.center, 0, _loginPanel);
        _loginPanel.rescale();

        // highscore panel
        _highscorePanel = new HighscorePanel(300, 300);
        _highscorePanel.setBackground("assets/images/background_panel.png");
        this.Center(UIPlacerMode.center, 0, _highscorePanel);
        _highscorePanel.rescale();
        
        _loginPanel.OnLogin += () => {
            b_login.Content = "Logout";
            tb_namemessage.Text = "You are logedin as " + Account.name;
            this.removeChild(_loginPanel);
        };

        string title = Account.isActivateAccount() ? "Logout" : "login";
        b_login = UIFactory.CreateButton(title, new Thickness(), new Point(100, 30), (x, y) =>{

            if (Account.isActivateAccount()) {
                Account.logout();
                b_login.Content = "Login";
                tb_namemessage.Text = "You are logedin as " + Account.name;
                return;
            }

            if (this.Children.Contains(_loginPanel)){
                this.removeChild(_loginPanel);
            }
            else{
                this.addChild(_loginPanel);
            }

        });
        this.CenterLeft(UIPlacerMode.top, 5, b_login);

        b_highscore = UIFactory.CreateButton("Highscore's", new Thickness(), new Point(70, 30), (x, y) =>{

            if (this.Children.Contains(_highscorePanel)){
                this.removeChild(_highscorePanel);
            }
            else{
                this.addChild(_highscorePanel);
            }

        });
        this.CenterRigth(UIPlacerMode.top, 5, b_highscore);

        // main menu
        tb_namemessage = UIFactory.CreateTextBlock("You are loged in as " + Account.name, new Thickness(), new Point(400, 50), 20);

        tb_ip = UIFactory.CreateTextBox(new Thickness(), new Point(200, 30), 20);

        b_client = UIFactory.CreateButton("Connect To Game", new Thickness(), new Point(200, 50), (x, y) => {
            startGame(false);
        });
        
        b_host = UIFactory.CreateButton("Host Game", new Thickness(), new Point(200, 50), (x, y) => {
            startGame(true);
        });

        b_quit = UIFactory.CreateButton("Quit", new Thickness(), new Point(200, 50), (x, y) => {
            System.Windows.Application.Current.Shutdown();
        });

        // lobby menu
        tb_lobbyname = UIFactory.CreateTextBlock(Account.name, new Thickness(), new Point(200, 30), 20);

        tb_lobbyscore = UIFactory.CreateTextBlock(string.Format("Wins: {0} / Losses: {1}", Account.score.wins, Account.score.losses), new Thickness(), new Point(200, 30), 20);

        tb_lobbygenredisplay = UIFactory.CreateTextBlock("Waiting for host", new Thickness(), new Point(200, 30), 20); // client only

        tb_lobbygenre = UIFactory.CreateTextBox(new Thickness(), new Point(200, 30), 20); // host only

        b_loggyconfirmgenre = UIFactory.CreateButton("Confirm Genre", new Thickness(), new Point(200, 50), (x, y) => { // host only
            _onGengreChange.send(tb_lobbygenre.Text);
        });

        b_lobbyback = UIFactory.CreateButton("Return To Menu", new Thickness(), new Point(200, 50), (x, y) => {
            initMenu();
        });

        b_lobbystartgame = UIFactory.CreateButton("Start Game", new Thickness(), new Point(200, 50), (x, y) => {
            _startGameCommand.send("");
        });

        tb_lobbyplayerdisplay = UIFactory.CreateTextBlock(Account.name + "\n", new Thickness(), new Point(300, 400), 20, TextAlignment.Left);
        tb_lobbyplayerdisplay.Background = Brushes.Gray;

        // loading screen
        tb_loadingmessage = UIFactory.CreateTextBlock("connection to game", new Thickness(), new Point(400, 30), 20);

        b_loadingback = UIFactory.CreateButton("Return To Menu", new Thickness(), new Point(200, 50), (x, y) => {
            terminateLobby();
        });
        this.Center(UIPlacerMode.center, 3, tb_loadingmessage, b_loadingback);

        initMenu();
    }

    /// <summary>
    /// init menu
    /// </summary>
    public void initMenu(){
        AudioManager.GetAudio("music_menu").Play(true);
        AudioManager.GetAudio("music_game").Stop();
        this.removeChild(tb_loadingmessage, b_loadingback);
        this.addChild(b_highscore);
        this.addChild(b_login);
        this.removeChild(_backgroundGame, tb_lobbyname, tb_lobbyscore, tb_lobbygenredisplay, tb_lobbygenre, b_loggyconfirmgenre, b_lobbyback, tb_lobbyplayerdisplay, b_lobbystartgame);

        this.addChild(tb_namemessage, tb_ip, b_client, b_host, b_quit);
        this.Center(UIPlacerMode.center, 3, tb_namemessage, tb_ip, b_client, b_host, b_quit);
    }

    /// <summary>
    /// init lobby
    /// </summary>
    public void initLobby(){
        _onGengreChange.activate();
        this.removeChild(tb_loadingmessage, b_loadingback);
        this.removeChild(b_highscore);
        this.removeChild(b_login);
        this.removeChild(_backgroundGame, tb_namemessage, tb_ip, b_client, b_host, b_quit);

        if (NetworkHandler.getInstance().isHost()){
            this.addChild(tb_lobbygenre, b_loggyconfirmgenre, b_lobbystartgame);

            this.CenterLeft(UIPlacerMode.center, 3, tb_lobbyname, tb_lobbyscore, tb_lobbygenre, b_loggyconfirmgenre, b_lobbyback, b_lobbystartgame);
        }
        else {
            this.addChild(tb_lobbygenredisplay);

            this.CenterLeft(UIPlacerMode.center, 3, tb_lobbyname, tb_lobbyscore, tb_lobbygenredisplay, b_lobbyback);
        }

        this.addChild(tb_lobbyname, tb_lobbyscore, b_lobbyback, tb_lobbyplayerdisplay);

        this.Center(UIPlacerMode.center, 3, tb_lobbyplayerdisplay);
    }

    /// <summary>
    /// init loading
    /// </summary>
    public void initLoading(){
        this.removeChild(b_highscore);
        this.removeChild(b_login);
        this.removeChild(_backgroundGame, tb_namemessage, b_lobbystartgame, tb_ip, b_client, b_host, b_quit, tb_lobbyname, tb_lobbyscore, tb_lobbygenredisplay, tb_lobbygenre, b_loggyconfirmgenre, b_lobbyback, tb_lobbyplayerdisplay);

        this.addChild(tb_loadingmessage, b_loadingback);
    }

    /// <summary>
    /// init none
    /// </summary>
    public void initNone() {
        AudioManager.GetAudio("music_game").Play(true);
        AudioManager.GetAudio("music_menu").Stop();
        this.Children.Clear();
        this.addChild(_backgroundGame);
        this.addChild(_notification);
    }
    
    /// <summary>
    /// start game
    /// </summary>
    /// <param name="_isHost">start game as host</param>
    public void startGame(bool _isHost){
        initLoading();

        if (_isHost){
            tb_loadingmessage.Text = "Trying to create lobby";
            
            try{
                NetworkManager.getInstance().create(NetworkType.Host, "", 8001, (x) => { // start host
                    this.Dispatcher.Invoke(() => { createLobby(); initLobby(); });
                });
            }
            catch (Exception){
                terminateLobby();
                return;
            }
            
        }
        else{
            IPAddress ip;
            if (!IPAddress.TryParse(tb_ip.Text, out ip)) {
                initMenu();
                // display error message (code from sander)
                return;
            }

            tb_loadingmessage.Text = "Trying to connect to lobby at " + tb_ip.Text;
            try{
                NetworkManager.getInstance().create(NetworkType.Client, tb_ip.Text, 8001, (x) => { // start client
                    this.Dispatcher.Invoke(() => { createLobby(); initLobby(); });
                });
            }catch (Exception){
                terminateLobby();
                // display error message
                return;
            }
            
        }

        NetworkHandler.getInstance().OnLostConnectionToHost += () => { // on lost connection with host callback
            MessageBox.Show("Lost connection with host!");
        };
    }

    /// <summary>
    /// terminate
    /// </summary>
    private void terminateLobby() {
        NetworkManager.getInstance().terminate();
        NetworkHandler.getInstance().terminate();
        lobbyManager = null;
        initMenu();
        _onGengreChange.disable();
    }

    /// <summary>
    /// generate lobby
    /// </summary>
    private void createLobby(){
        lobbyManager = new LobbyManager(Account.name);
        tb_lobbyplayerdisplay.Text = Account.name + "\n";
        
        lobbyManager.OnPlayerJoin += (x) => {
            this.Dispatcher.Invoke(() => {
                tb_lobbyplayerdisplay.Text += x + "\n";

                if (NetworkHandler.getInstance().isHost()) {
                    _onGengreChange.send(tb_lobbygenre.Text);
                }
            });
        };

        lobbyManager.init();

        lobbyManager.OnStart += (x) => {
            string genre = tb_lobbygenredisplay.Text != "" ? tb_lobbygenredisplay.Text : "cute cats";
            gamepanel = new GamePanel(5, 4, 120, 200, genre); // init the game panel
            this.addChild(gamepanel);

            List<PlayerInfo> playerInfo = new List<PlayerInfo>();
            for (int i = 0; i < x.Length; i++){
                PlayerInfo panel = new PlayerInfo(x[i].name, new Point(300, 70), (PlayerInfo.ScreenLocation)i, 20);
                this.addChild(panel);
                playerInfo.Add(panel);
            }

            turnManager = new TurnManager(x, playerInfo, gamepanel);
        };
    }

}
