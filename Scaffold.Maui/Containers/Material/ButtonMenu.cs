using ButtonSam.Maui.Core;
using ScaffoldLib.Maui.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.Material;

/// <summary>
/// Button menu on navigation bar (non collapsed)
/// </summary>
internal class ButtonMenu : Internal.Button
{
    private ContentType currentContentType = ContentType.None;

    #region bindable props
    // image source
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource), 
        typeof(ImageSource),
        typeof(ButtonMenu),
        null,
        propertyChanged: (b,o,n) => 
        {
            if (b is ButtonMenu self) self.Update(); 
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
        typeof(ButtonMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self) self.Update();
        }
    );
    public string? Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    // foreground color
    public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(
        nameof(ForegroundColor),
        typeof(Color),
        typeof(ButtonMenu),
        Colors.Black,
        propertyChanged:(b,o,n) =>
        {
            if (b is ButtonMenu self)
                self.UpdateColor();
        }
    );
    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }

    // Menu item color
    public static readonly BindableProperty MenuItemColorProperty = BindableProperty.Create(
        nameof(MenuItemColor),
        typeof(Color),
        typeof(ButtonMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
                self.UpdateColor();
        }
    );
    public Color? MenuItemColor
    {
        get => GetValue(MenuItemColorProperty) as Color;
        set => SetValue(MenuItemColorProperty, value);
    }


    // use original color
    public static readonly BindableProperty UseOriginalColorProperty = BindableProperty.Create(
        nameof(UseOriginalColor),
        typeof(bool),
        typeof(ButtonMenu),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
                self.UpdateColor();
        }
    );
    public bool UseOriginalColor
    {
        get => (bool)GetValue(UseOriginalColorProperty);
        set => SetValue(UseOriginalColorProperty, value);
    }
    #endregion bindable props

    //protected override void HandleInteractiveRunning(HandleInteractiveRunningArgs args)
    //{
    //    if (args.Input.X < 0 || args.Input.X > Width)
    //    {
    //        args.IsPressed = false;
    //        return;
    //    }

    //    if (args.Input.Y < 0 || args.Input.Y > Height)
    //    {
    //        args.IsPressed = false;
    //        return;
    //    }
    //}

    private void Update()
    {
        var oldValue = currentContentType;
        ContentType newValue;

        if (ImageSource != null)
            newValue = ContentType.Icon;
        else if (Text != null)
            newValue = ContentType.Text;
        else
            newValue = ContentType.None;

        currentContentType = newValue;

        if (oldValue != newValue)
        {
            switch (newValue)
            {
                case ContentType.Icon:
                    Padding = new Thickness(8);
                    BackgroundColor = Colors.Transparent;
                    Content = new ImageTint
                    {
                        WidthRequest = 24,
                        HeightRequest = 24,
                        Source = ImageSource,
                    };
                    CornerRadius = 20;
                    break;

                case ContentType.Text:
                    Padding = new Thickness(10);
                    Content = new Label
                    {
                        Text = Text,
                    };
                    CornerRadius = 8;
                    break;

                default:
                    Content = null;
                    return;
            }
            UpdateColor();
        }
        else
        {
            switch (newValue)
            {
                case ContentType.Icon:
                    if (Content is ImageTint img)
                        img.Source = ImageSource;
                    break;
                case ContentType.Text:
                    if (Content is Label label)
                        label.Text = Text;
                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateColor()
    {
        switch (Content)
        {
            case ImageTint img:
                var iconColor = MenuItemColor ?? ForegroundColor;
                if (UseOriginalColor)
                    iconColor = null;

                img.TintColor = iconColor;
                break;
            case Label label:
                label.TextColor = MenuItemColor ?? ForegroundColor;
                break;
            default:
                break;
        }
    }

    private enum ContentType
    {
        None,
        Icon,
        Text,
    }
}
