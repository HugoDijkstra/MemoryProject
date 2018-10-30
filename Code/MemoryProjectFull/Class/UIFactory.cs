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

public class UIFactory {

    /// <summary>
    /// create button
    /// </summary>
    /// <param name="_message">title</param>
    /// <param name="_margin">position</param>
    /// <param name="_size">size</param>
    /// <param name="_callback">callback</param>
    /// <returns>created button</returns>
    public static Button CreateButton(string _message, Thickness _margin, Point _size, Action<object, RoutedEventArgs> _callback) {
        Button b = new Button();
        b.Content = _message;
        b.Margin = _margin;
        b.Width = _size.X;
        b.Height = _size.Y;
        b.Click += (x, y) => { _callback(x, y); AudioManager.GetAudio("button_click").Play(false); };
        b.Background = new SolidColorBrush(Color.FromArgb(125, 0, 0, 0));
        b.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        b.BorderThickness = new Thickness();


        return b;
    }

    /// <summary>
    /// create text block
    /// </summary>
    /// <param name="_message">message</param>
    /// <param name="_margin">position</param>
    /// <param name="_size">size</param>
    /// <param name="_fontSize">font size</param>
    /// <param name="_alignment">alignment</param>
    /// <returns>created text block</returns>
    public static TextBlock CreateTextBlock(string _message, Thickness _margin, Point _size, int _fontSize, TextAlignment _alignment = TextAlignment.Center) {
        TextBlock t = new TextBlock();
        t.Text = _message;
        t.Margin = _margin;
        t.Width = _size.X;
        t.Height = _size.Y;
        t.FontSize = _fontSize;
        t.TextAlignment = _alignment;
        t.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); ;

        return t;
    }

    /// <summary>
    /// create text box
    /// </summary>
    /// <param name="_margin">position</param>
    /// <param name="_size">size</param>
    /// <param name="_fontSize">font size</param>
    /// <param name="_alignment">alignment</param>
    /// <returns></returns>
    public static TextBox CreateTextBox(Thickness _margin, Point _size, int _fontSize, TextAlignment _alignment = TextAlignment.Center) {
        TextBox t = new TextBox();
        t.Margin = _margin;
        t.Width = _size.X;
        t.Height = _size.Y;
        t.FontSize = _fontSize;
        t.TextAlignment = _alignment;
        t.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); ;

        return t;
    }

    /// <summary>
    /// create password box
    /// </summary>
    /// <param name="_margin">position</param>
    /// <param name="_size">size</param>
    /// <param name="_fontSize">font size</param>
    /// <returns>created password box</returns>
    public static PasswordBox CreatePasswordBox(Thickness _margin, Point _size, int _fontSize) {
        PasswordBox t = new PasswordBox();
        t.Margin = _margin;
        t.Width = _size.X;
        t.Height = _size.Y;
        t.FontSize = _fontSize;
        t.PasswordChar = '\u25CF';
        t.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); ;

        return t;
    }

    /// <summary>
    /// create check box
    /// </summary>
    /// <param name="_message"></param>
    /// <param name="_margin"></param>
    /// <param name="_size"></param>
    /// <param name="_autoToggle"></param>
    /// <returns>created check box</returns>
    public static CheckBox CreateCheckBox(string _message, Thickness _margin, Point _size, bool _autoToggle = false) {
        CheckBox c = new CheckBox();
        c.Content = _message;
        c.Margin = _margin;
        c.Width = _size.X;
        c.Height = _size.Y;
        c.IsChecked = _autoToggle;
        c.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); ;

        return c;
    }
}

public enum UIPlacerMode {
    top,
    center,
    bottom
}