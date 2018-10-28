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

class Menu : Canvas {

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

    public Menu() : base() {

        _startGameCommand = new NetworkCommand("G:START", (x) => {
            this.Dispatcher.Invoke(() => { lobbyManager.startGame(); });
        }, false, true);

        _panel = new LoginPanel(300, 500);
        _panel.setBackground(Brushes.Gray);
        UIPlacer.Center(UIPlacerMode.center, 0, _panel);

        b_login = UIFactory.CreateButton("LOGIN", new Thickness(), new Point(70, 30), (x, y) => {

            if (this.Children.Contains(_panel)){
                this.Children.Remove(_panel);
            }
            else {
                this.Children.Add(_panel);
            }
            
        });
        this.Children.Add(b_login);

        UIPlacer.CenterLeft(UIPlacerMode.top, 5, b_login);

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

            UIPlacer.Center(UIPlacerMode.center, 3, tb_nameText, b_host, b_client);
        });

        this.Children.Add(b_name);
        UIPlacer.Center(UIPlacerMode.center, 3, tb_nameInput, b_name);

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

            gamepanel = new GamePanel(5, 4, 100, 200, "cats"); // init the game panel
            this.Children.Add(gamepanel);

            turnManager = new TurnManager(x, gamepanel);
        };

        if (NetworkHandler.getInstance().isHost()) {
            this.Children.Add(b_start);
            UIPlacer.Center(UIPlacerMode.center, 3, b_start);
        }
    }

}
