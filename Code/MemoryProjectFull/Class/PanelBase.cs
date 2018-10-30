using MemoryProjectFull;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

public class PanelBase : Canvas {

    /// <summary>
    /// panel base 
    /// </summary>
    /// <param name="_width">panel width</param>
    /// <param name="_height">panel height</param>
    public PanelBase(int _width, int _height) : base() {
        this.Width = _width;
        this.Height = _height;
    }

    /// <summary>
    /// rescale function
    /// </summary>
    public virtual void rescale() { }

    /// <summary>
    /// set background
    /// </summary>
    /// <param name="_path">file path</param>
    public void setBackground(string _path) {
        this.Background = new ImageBrush() { ImageSource = new BitmapImage((new Uri(_path, UriKind.RelativeOrAbsolute))) };
    }
    /// <summary>
    /// set background
    /// </summary>
    /// <param name="_color">color</param>
    public void setBackground(SolidColorBrush _color) {
        this.Background = _color;
    }
    /// <summary>
    /// remove background
    /// </summary>
    public void removeBackground() {
        this.Background = null;
    }

    /// <summary>
    /// add children
    /// </summary>
    /// <param name="_elements">children to add</param>
    public void addChild(params FrameworkElement[] _elements) {
        for (int i = 0; i < _elements.Length; i++){
            this.Children.Add(_elements[i]);
        }
    }
    /// <summary>
    /// remove children
    /// </summary>
    /// <param name="_elements">children to remove</param>
    public void removeChild(params FrameworkElement[] _elements) {
        for (int i = 0; i < _elements.Length; i++){
            this.Children.Remove(_elements[i]);
        }
    }

    /// <summary>
    /// center ui
    /// </summary>
    /// <param name="_mode">center mode</param>
    /// <param name="_padding">padding</param>
    /// <param name="_elements">element to center</param>
    public void Center(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements){
        int startHeight = _mode == UIPlacerMode.top ? ((int)_elements[0].Height) / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = MainWindow.SCREEN_WIDTH / 2;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness((startWidth - (_elements[i].Width / 2)) - this.Margin.Left, startHeight - this.Margin.Top, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    /// <summary>
    /// center ui
    /// </summary>
    /// <param name="_mode">center mode</param>
    /// <param name="_padding">padding</param>
    /// <param name="_elements">element to center</param>
    public void CenterLeft(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements) {
        int startHeight = _mode == UIPlacerMode.top ? (int)_elements[0].Height / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = 0;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness(startWidth + _padding - this.Margin.Left, startHeight - this.Margin.Top, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    /// <summary>
    /// center ui
    /// </summary>
    /// <param name="_mode">center mode</param>
    /// <param name="_padding">padding</param>
    /// <param name="_elements">element to center</param>
    public void CenterRigth(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements) {
        int startHeight = _mode == UIPlacerMode.top ? (int)_elements[0].Height / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = MainWindow.SCREEN_WIDTH;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness((startWidth - (_elements[i].Width + _padding)) - this.Margin.Left, startHeight - this.Margin.Top, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    /// <summary>
    /// get next height of element
    /// </summary>
    /// <param name="_height">current height</param>
    /// <param name="_mode">center mode</param>
    /// <param name="_element">element</param>
    /// <param name="_padding">padding</param>
    /// <returns>next height</returns>
    private int getNextHeight(int _height, UIPlacerMode _mode, FrameworkElement _element, int _padding = 0) {
        if (_mode == UIPlacerMode.top || _mode == UIPlacerMode.center) {
            return _height + (int)_element.Height + _padding;
        }
        return _height - (int)_element.Height - _padding;
    }

    /// <summary>
    /// get total height
    /// </summary>
    /// <param name="_elements">elements</param>
    /// <param name="_padding">padding</param>
    /// <returns>total height</returns>
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

