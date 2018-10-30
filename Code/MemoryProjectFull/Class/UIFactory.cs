using MemoryProjectFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

public class UIFactory {

    public static Button CreateButton(string _message, Thickness _margin, Point _size, Action<object, RoutedEventArgs> _callback) {
        Button b = new Button();
        b.Content = _message;
        b.Margin = _margin;
        b.Width = _size.X;
        b.Height = _size.Y;
        b.Click += (x, y) => { _callback(x, y); };

        return b;
    }

    public static TextBlock CreateTextBlock(string _message, Thickness _margin, Point _size, int _fontSize, TextAlignment _alignment = TextAlignment.Center) {
        TextBlock t = new TextBlock();
        t.Text = _message;
        t.Margin = _margin;
        t.Width = _size.X;
        t.Height = _size.Y;
        t.FontSize = _fontSize;
        t.TextAlignment = _alignment;

        return t;
    }

    public static TextBox CreateTextBox(Thickness _margin, Point _size, int _fontSize, TextAlignment _alignment = TextAlignment.Center) {
        TextBox t = new TextBox();
        t.Margin = _margin;
        t.Width = _size.X;
        t.Height = _size.Y;
        t.FontSize = _fontSize;
        t.TextAlignment = _alignment;

        return t;
    }

    public static PasswordBox CreatePasswordBox(Thickness _margin, Point _size, int _fontSize) {
        PasswordBox t = new PasswordBox();
        t.Margin = _margin;
        t.Width = _size.X;
        t.Height = _size.Y;
        t.FontSize = _fontSize;
        t.PasswordChar = '\u25CF';

        return t;
    }

    public static CheckBox CreateCheckBox(string _message, Thickness _margin, Point _size, bool _autoToggle = false) {
        CheckBox c = new CheckBox();
        c.Content = _message;
        c.Margin = _margin;
        c.Width = _size.X;
        c.Height = _size.Y;
        c.IsChecked = _autoToggle;

        return c;
    }
}

public enum UIPlacerMode {
    top,
    center,
    bottom
}

public class UIPlacer {
    public static void Center(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements) {
        int startHeight = _mode == UIPlacerMode.top ? (int)_elements[0].Height / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = MainWindow.SCREEN_WIDTH / 2;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness(startWidth - (_elements[i].Width/2), startHeight, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }
    public static void Center(UIPlacerMode _mode, int _padding, int _widthOffset, params FrameworkElement[] _elements) {
        int startHeight = _mode == UIPlacerMode.top ? (int)_elements[0].Height / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = MainWindow.SCREEN_WIDTH / 2 + _widthOffset;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness(startWidth - (_elements[i].Width/2), startHeight, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    public static void CenterLeft(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements) {
        int startHeight = _mode == UIPlacerMode.top ? (int)_elements[0].Height / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = 0;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness(startWidth + _padding, startHeight, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    public static void CenterRigth(UIPlacerMode _mode, int _padding, params FrameworkElement[] _elements) {
        int startHeight = _mode == UIPlacerMode.top ? (int)_elements[0].Height / 2 + _padding : _mode == UIPlacerMode.center ? MainWindow.SCREEN_HEIGHT / 2 - getTotalHeight(_elements, _padding) / 2 : MainWindow.SCREEN_HEIGHT - (int)_elements[0].Height / 2 - _padding;
        int startWidth = MainWindow.SCREEN_WIDTH;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness(startWidth - (_elements[i].Width + _padding), startHeight, 0, 0);
            _elements[i].Margin = th;
            startHeight = getNextHeight(startHeight, _mode, _elements[i], _padding);
        }
    }

    private static int getNextHeight(int _height, UIPlacerMode _mode, FrameworkElement _element, int _padding = 0) {
        if (_mode == UIPlacerMode.top || _mode == UIPlacerMode.center) {
            return _height + (int)_element.Height + _padding;
        }
        return _height - (int)_element.Height - _padding;
    }

    private static int getTotalHeight(FrameworkElement[] _elements, int _padding = 0) {
        int height = 0;
        for (int i = 0; i < _elements.Length; i++){
            height += (int)_elements[i].Height;

            if (i + 1 < _elements.Length)
                height += _padding;
        }
        return height;
    }
}
