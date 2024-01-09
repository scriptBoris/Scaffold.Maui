using Microsoft.Maui.Controls.Platform;
using ScaffoldLib.Maui.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleDll.Controls;

public class MenuButton : ButtonSam.Maui.Button
{
    public MenuButton()
    {
        label = new Label
        {
            VerticalTextAlignment = TextAlignment.Center,
        };
        image = new ImageTint
        {
            WidthRequest = 18,
            HeightRequest = 18,
        };
        image.SetAppThemeColor(ImageTint.TintColorProperty, Colors.Gray, Colors.White);

        _stack = new StackLayout
        {
            Spacing = 20,
            Orientation = StackOrientation.Horizontal,
        };
        _stack.Children.Add(image);
        _stack.Children.Add(label);
        Content = _stack;
        CornerRadius = 6;

#if WINDOWS
        Padding = 6;
#else
        Padding = 10;
        image.WidthRequest = 28;
        image.HeightRequest = 28;
        label.FontSize = 18;
#endif
        UpdateIsSelected();
    }

    private readonly StackLayout _stack;
    private Label label;
    private ImageTint image;

    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MenuButton),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuButton self)
                self.label.Text = n as string;
        }
    );
    public string? Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    // image
    public static readonly BindableProperty ImageProperty = BindableProperty.Create(
        nameof(Image),
        typeof(ImageSource),
        typeof(MenuButton),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuButton self)
                self.image.Source = n as ImageSource;
        }
    );
    public ImageSource? Image
    {
        get => GetValue(ImageProperty) as ImageSource;
        set => SetValue(ImageProperty, value);
    }

    // is selected
    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
        nameof(IsSelected),
        typeof(bool),
        typeof(MenuButton),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuButton self)
                self.UpdateIsSelected();
        }
    );
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var res = base.MeasureOverride(widthConstraint, heightConstraint);

        if (widthConstraint <= 40)
        {
            label.Opacity = 0;
        }
        else
        {
            double opacity = widthConstraint / 150;
            if (opacity > 1)
                opacity = 1;
            label.Opacity = opacity;
        }

        return res;
    }

    private void UpdateIsSelected()
    {
        this.Dispatcher.Dispatch(() =>
        {
            var def = Colors.Gray.WithAlpha(0.2f);
            if (IsSelected)
            {
                this.BackgroundColor = def;
            }
            else
            {
                BackgroundColor = new Color(0, 0, 0, 1);
            }
        });
    }
}
