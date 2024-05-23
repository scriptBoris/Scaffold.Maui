using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MenuItemCollection = ScaffoldLib.Maui.Core.MenuItemCollection;

namespace ScaffoldLib.Maui;

public class ScaffoldMenuItem : BindableObject
{
    private MenuItemCollection? parent;

    #region bindable props
    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ScaffoldMenuItem),
        null
    );
    public string? Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    // image source
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(ScaffoldMenuItem),
        null
    );
    public ImageSource? ImageSource
    {
        get => GetValue(ImageSourceProperty) as ImageSource;
        set => SetValue(ImageSourceProperty, value);
    }

    // color
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(ScaffoldMenuItem),
        null
    );
    public Color? Color
    {
        get => GetValue(ColorProperty) as Color;
        set => SetValue(ColorProperty, value);
    }

    // use original color
    public static readonly BindableProperty UseOriginalColorProperty = BindableProperty.Create(
        nameof(UseOriginalColor),
        typeof(bool),
        typeof(ScaffoldMenuItem),
        false
    );
    public bool UseOriginalColor
    {
        get => (bool)GetValue(UseOriginalColorProperty);
        set => SetValue(UseOriginalColorProperty, value);
    }

    // command
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command),
        typeof(ICommand),
        typeof(ScaffoldMenuItem),
        null
    );
    public ICommand? Command
    {
        get => GetValue(CommandProperty) as ICommand;
        set => SetValue(CommandProperty, value);
    }

    // is collapsed
    public static readonly BindableProperty IsCollapsedProperty = BindableProperty.Create(
        nameof(IsCollapsed),
        typeof(bool),
        typeof(ScaffoldMenuItem),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is ScaffoldMenuItem self)
                self.parent?.ResolveItem(self, self.IsVisible, (bool)o);
        }
    );
    public bool IsCollapsed
    {
        get => (bool)GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }

    // is visible
    public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(
        nameof(IsVisible),
        typeof(bool),
        typeof(ScaffoldMenuItem),
        true,
        propertyChanged: (b, o, n) =>
        {
            if (b is ScaffoldMenuItem self)
                self.parent?.ResolveItem(self, (bool)o, self.IsCollapsed);
        }
    );
    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    // image source padding
    public static readonly BindableProperty ImageSourcePaddingProperty = BindableProperty.Create(
        nameof(ImageSourcePadding),
        typeof(Thickness),
        typeof(ScaffoldMenuItem),
#if ANDROID
        new Thickness(8)
#elif IOS
        new Thickness(10)
#else
        new Thickness(8)
#endif
    );
    public Thickness ImageSourcePadding
    {
        get => (Thickness)GetValue(ImageSourcePaddingProperty);
        set => SetValue(ImageSourcePaddingProperty, value);
    }
#endregion bindable props

    internal void SetParent(MenuItemCollection? parent)
    {
        this.parent = parent;
    }
}