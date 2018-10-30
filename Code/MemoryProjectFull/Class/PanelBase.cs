using MemoryProjectFull;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

public class PanelBase : Canvas {

    public PanelBase(int _width, int _height) : base() {
        this.Width = _width;
        this.Height = _height;
    }

    // rescaling
    public virtual void rescale() { }

    // backgrounds
    public void setBackground(string _path) {
        this.Background = new ImageBrush() { ImageSource = new BitmapImage((new Uri(_path, UriKind.RelativeOrAbsolute))) };
    }
    public void setBackground(SolidColorBrush _color) {
        this.Background = _color;
    }
    public void removeBackground() {
        this.Background = null;
    }

    // childeren
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

    // scaling
    public void Center(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements){
        int startHeight = _mode == UIPlacerMode.top ? ((int)_elements[0].Height) / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = MainWindow.SCREEN_WIDTH / 2;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness((startWidth - (_elements[i].Width / 2)) - this.Margin.Left, startHeight - this.Margin.Top, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    public void CenterLeft(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements) {
        int startHeight = _mode == UIPlacerMode.top ? (int)_elements[0].Height / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = 0;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness(startWidth + _padding - this.Margin.Left, startHeight - this.Margin.Top, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    public void CenterRigth(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements) {
        int startHeight = _mode == UIPlacerMode.top ? (int)_elements[0].Height / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = MainWindow.SCREEN_WIDTH;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness((startWidth - (_elements[i].Width + _padding)) - this.Margin.Left, startHeight - this.Margin.Top, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    // extra scaling fucntions
    private int getNextHeight(int _height, UIPlacerMode _mode, FrameworkElement _element, int _padding = 0) {
        if (_mode == UIPlacerMode.top || _mode == UIPlacerMode.center) {
            return _height + (int)_element.Height + _padding;
        }
        return _height - (int)_element.Height - _padding;
    }
    private int getTotalHeight(FrameworkElement[] _elements, int _padding = 0) {
        int height = 0;
        for (int i = 0; i < _elements.Length; i++){
            height += (int)_elements[i].Height;

            if (i + 1 < _elements.Length)
                height += _padding;
        }
        return height;
    }
}

public class LoginPanel : PanelBase {

    public Action OnLogin;

    private bool isSwitched;

    private TextBlock tb_message;

    private TextBox tb_name;
    private PasswordBox tb_password;
    private CheckBox cb_autologin;
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

        cb_autologin = UIFactory.CreateCheckBox("Save login credentials?", new Thickness(), new Point(250, 15));

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
                this.addChild(tb_name, tb_password, cb_autologin, b_login);
                this.Center(UIPlacerMode.center, 3, tb_message, tb_name, tb_password, cb_autologin, b_login, b_switch); // <-- used to center stuf on screen
            } else {
                tb_message.Text = "Register a new account";
                b_switch.Content = "Login To Existing Account";
                this.removeChild(tb_name, tb_password, cb_autologin, b_login);
                this.addChild(tb_reg_name, tb_reg_password, b_register);
                this.Center(UIPlacerMode.center, 3, tb_message, tb_reg_name, tb_reg_password, b_register, b_switch); // <-- used to center stuf on screen
            }

            isSwitched = !isSwitched;
        });

        this.addChild(tb_name, tb_password, b_login, b_switch, tb_message, cb_autologin);
        this.Center(UIPlacerMode.center, 3, tb_message, tb_name, tb_password, cb_autologin, b_login, b_switch); // <-- used to center stuf on screen
    }

    public override void rescale()
    {
        if (isSwitched)
            this.Center(UIPlacerMode.center, 3, tb_message, tb_reg_name, tb_reg_password, b_register, b_switch); // <-- used to center stuf on screen
        else
            this.Center(UIPlacerMode.center, 3, tb_message, tb_name, tb_password, cb_autologin, b_login, b_switch); // <-- used to center stuf on screen
    }

    private void login() {
        Account.login(tb_name.Text, tb_password.Password, cb_autologin.IsChecked.Value, OnLogin);
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
