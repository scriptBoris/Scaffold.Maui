using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.Material;

/// <summary>
/// Button menu on popup dialog (show collapsed menu items)
/// </summary>
internal class ButtonMenuOnPopup : ButtonSam.Maui.Button
{
    private readonly ImageTint _icon;
    private readonly Label _label;

    public ButtonMenuOnPopup()
    {
        var stack = new HorizontalStackLayout
        {
            Spacing = 10,
        };

        _icon = new ImageTint
        {
            HeightRequest = 24,
            WidthRequest = 24,
        };
        stack.Children.Add(_icon);

        _label = new Label
        {
            Margin = new Thickness(0, 0, 10, 0),
            VerticalTextAlignment = TextAlignment.Center,
        };
        stack.Children.Add(_label);
        Content = stack;

        UpdateForeground();
    }

    // image source
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(ButtonMenuOnPopup),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenuOnPopup self) self._icon.Source = n as ImageSource;
        }
    );
    public ImageSource? ImageSource
    {
        get => GetValue(ImageSourceProperty) as ImageSource;
        set => SetValue(ImageSourceProperty, value);
    }

    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ButtonMenuOnPopup),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenuOnPopup self) self._label.Text = n as string;
        }
    );
    public string? Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    // use original color
    public static readonly BindableProperty UseOriginalColorProperty = BindableProperty.Create(
        nameof(UseOriginalColor),
        typeof(bool),
        typeof(ButtonMenuOnPopup),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenuOnPopup self) self.UpdateForeground();
        }
    );
    public bool UseOriginalColor
    {
        get => (bool)GetValue(UseOriginalColorProperty);
        set => SetValue(UseOriginalColorProperty, value);
    }

    private void UpdateForeground()
    {
        if (UseOriginalColor)
            _icon.TintColor = null;
        else
            _icon.SetAppThemeColor(ImageTint.TintColorProperty, Color.FromRgba("#444"), Color.FromRgba("#CCC"));

        _label.SetAppThemeColor(Label.TextColorProperty, Color.FromRgba("#111"), Color.FromRgba("#FFF"));
    }
}
