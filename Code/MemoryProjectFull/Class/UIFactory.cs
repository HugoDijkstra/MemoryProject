using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

    public static void Center(int _screenWidth, int _topOffset, int _padding, params FrameworkElement[] _elements) {
        int startPos = _topOffset;
        for (int i = 0; i < _elements.Length; i++){
            Thickness th = new Thickness(_screenWidth / 2 - (_elements[i].Width/2), startPos, 0, 0);
            _elements[i].Margin = th;
            startPos += (int)_elements[i].Height + _padding;
        }
    }
}
