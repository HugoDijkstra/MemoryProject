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

    private TextBox tb_name;
    private PasswordBox tb_password;
    private Button b_login;

    public LoginPanel(int _width, int _height) : base(_width, _height) {
        tb_name = UIFactory.CreateTextBox(new Thickness(), new Point(250, 30), 20, TextAlignment.Left); // <-- used to create buttons, textboxs and more ( based of a factory google can tell how it workes [fewwy easy])
        tb_password = UIFactory.CreatePasswordBox(new Thickness(), new Point(250, 30), 20);
        

        b_login = UIFactory.CreateButton("Login", new Thickness(), new Point(250, 50), (x, y) =>{
            // callback
        });

        this.addChild(tb_name, tb_password, b_login);
        UIPlacer.Center(UIPlacerMode.center, 3, -(MainWindow.SCREEN_WIDTH/2 - 150), tb_name, tb_password, b_login); // <-- used to center stuf on screen
    }
}
