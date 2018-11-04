using MemoryProjectFull;
using NotificationsWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

    /// <summary>
    /// login panel contructor
    /// </summary>
    /// <param name="_width">panel width</param>
    /// <param name="_height">panel height</param>
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

        b_register = UIFactory.CreateButton("Regist", new Thickness(), new Point(250, 50), (x, y) => {
            regist();
        });

        b_switch = UIFactory.CreateButton("Register a new account", new Thickness(), new Point(250, 30), (x, y) => {
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

    /// <summary>
    /// rescale ui
    /// </summary>
    public override void rescale()
    {
        if (isSwitched)
            this.Center(UIPlacerMode.center, 3, tb_message, tb_reg_name, tb_reg_password, b_register, b_switch); // <-- used to center stuf on screen
        else
            this.Center(UIPlacerMode.center, 3, tb_message, tb_name, tb_password, cb_autologin, b_login, b_switch); // <-- used to center stuf on screen
    }

    /// <summary>
    /// login account
    /// </summary>
    private void login() {
        if (tb_name.Text == string.Empty || tb_password.Password == string.Empty) {
            NotificationManager.RequestNotification("You entered no name or password, please try again!");
            return;
        }

        if (Regex.IsMatch(tb_name.Text, @"^[\%\/\\\&\?\,\'\;\:\!\-]+$")){
            NotificationManager.RequestNotification("Name cant contain any special chars, please try again!");
            return;
        }

        if (Account.login(tb_name.Text, tb_password.Password, cb_autologin.IsChecked.Value, OnLogin)) {
            NotificationManager.RequestNotification("You are loged in as " + Account.name);
            return;
        }
    }

    /// <summary>
    /// regist account
    /// </summary>
    private void regist() {

        if (tb_reg_name.Text == string.Empty || tb_reg_password.Password == string.Empty) {
            // error message
            return;
        }

        if (Regex.IsMatch("", @"^[\%\/\\\&\?\,\'\;\:\!\-]+$")){
            // error message
            return;
        }

        if (MemoryDatabase.database.CheckTableExistence("users") && !MemoryDatabase.database.TableContainsData("users", "name", "'" + tb_reg_name.Text + "'")) {
            SortedList<string, string> userData = new SortedList<string, string>();
            userData.Add("id", "0");
            userData.Add("name", tb_reg_name.Text);
            userData.Add("password", tb_reg_password.Password);
            userData.Add("wins", "0");
            userData.Add("losses", "0");
            MemoryDatabase.database.AddDataToTable("users", userData);
            Account.login(tb_reg_name.Text, tb_reg_password.Password, false, OnLogin);
        }else {
            // error message
            return;
        }
    }
}