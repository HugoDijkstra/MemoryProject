using MemoryProjectFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public class PanelBase : Canvas{

    public PanelBase(int _width, int _height) : base() {
        this.Width = _width;
        this.Height = _height;
    }

    public void setBackground(string _path) {
        this.Background = new ImageBrush() { ImageSource = new BitmapImage((new Uri(_path, UriKind.Absolute))) };
    }
    public void setBackground(SolidColorBrush _color) {
        this.Background = _color;
    }
    public void removeBackground() {
        this.Background = null;
    }

    public void addChild(params FrameworkElement[] _elements) {
        for (int i = 0; i < _elements.Length; i++){
            this.Children.Add(_elements[i]);
        }
    }
    public void removeChild(params FrameworkElement[] _elements) {
        for (int i = 0; i < _elements.Length; i++){
            this.Children.Remove(_elements[i]);
        }
    }

}

public class LoginPanel : PanelBase {

    private bool isSwitched;

    private TextBlock tb_message;

    private TextBox tb_name;
    private PasswordBox tb_password;
    private Button b_login;

    private TextBox tb_reg_name;
    private PasswordBox tb_reg_password;
    private Button b_register;


    private Button b_switch;

    public LoginPanel(int _width, int _height) : base(_width, _height) {
        isSwitched = false;

        tb_message = UIFactory.CreateTextBlock("Login to your acount", new Thickness(), new Point(250, 60), 20);

        tb_name = UIFactory.CreateTextBox(new Thickness(), new Point(250, 30), 20, TextAlignment.Left); // <-- used to create buttons, textboxs and more ( based of a factory google can tell how it workes [fewwy easy])
        tb_password = UIFactory.CreatePasswordBox(new Thickness(), new Point(250, 30), 20);
        
        b_login = UIFactory.CreateButton("Login", new Thickness(), new Point(250, 50), (x, y) =>{
            login();
        });

        tb_reg_name = UIFactory.CreateTextBox(new Thickness(), new Point(250, 30), 20, TextAlignment.Left); // <-- used to create buttons, textboxs and more ( based of a factory google can tell how it workes [fewwy easy])
        tb_reg_password = UIFactory.CreatePasswordBox(new Thickness(), new Point(250, 30), 20);

        b_register = UIFactory.CreateButton("Register", new Thickness(), new Point(250, 50), (x, y) => {
            regist();
        });

        b_switch = UIFactory.CreateButton("Login To Existing Account", new Thickness(), new Point(250, 30), (x, y) => {
            if (isSwitched){
                tb_message.Text = "Login to your account";
                b_switch.Content = "Regist New Account";
                this.removeChild(tb_reg_name, tb_reg_password, b_register);
                this.addChild(tb_name, tb_password, b_login);
            } else {
                tb_message.Text = "Register a new account";
                b_switch.Content = "Login To Existing Account";
                this.removeChild(tb_name, tb_password, b_login);
                this.addChild(tb_reg_name, tb_reg_password, b_register);
            }

            isSwitched = !isSwitched;
        });

        this.addChild(tb_name, tb_password, b_login, b_switch, tb_message);
        UIPlacer.Center(UIPlacerMode.center, 3, -(MainWindow.SCREEN_WIDTH/2 - 150), tb_message, tb_name, tb_password, b_login, b_switch); // <-- used to center stuf on screen
        UIPlacer.Center(UIPlacerMode.center, 3, -(MainWindow.SCREEN_WIDTH / 2 - 150), tb_message, tb_reg_name, tb_reg_password, b_register, b_switch); // <-- used to center stuf on screen
    }

    private void login() {
        Account.login(tb_name.Text, tb_password.Password);
    }

    private void regist() {
        if (MemoryDatabase.database.CheckTableExistence("users") && !MemoryDatabase.database.TableContainsData("users", "name", "'" + tb_reg_name.Text + "'")) {
            SortedList<string, string> userData = new SortedList<string, string>();
            userData.Add("id", "0");
            userData.Add("name", tb_reg_name.Text);
            userData.Add("password", tb_reg_password.Password);
            userData.Add("wins", "0");
            userData.Add("loses", "0");
            MemoryDatabase.database.AddDataToTable("users", userData);
        }
    }
}
