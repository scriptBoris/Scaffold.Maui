﻿using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.Material;

public class ButtonMenu : ButtonSam.Maui.Button
{
    // image source
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource), 
        typeof(ImageSource),
        typeof(ButtonMenu),
        null,
        propertyChanged: Update
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
        propertyChanged: Update
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
        Colors.White,
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

    private static void Update(BindableObject b, object old, object newest)
    {
        if (b is ButtonMenu self)
        {
            self.Update();
        }
    }

    private void Update()
    {
        if (ImageSource != null)
        {
            Padding = new Thickness(5);
            BackgroundColor = Colors.Transparent;
            Content = new ImageTint
            {
                WidthRequest = 26,
                HeightRequest = 26,
                Source = ImageSource,
            };
            CornerRadius = 18;
        }
        else
        {
            Padding = new Thickness(10);
            Content = new Label
            {
                Text = Text,
            };
            CornerRadius = 8;
        }
    }

    private void UpdateColor()
    {
        var color = MenuItemColor ?? ForegroundColor;

        if (UseOriginalColor)
            color = null;

        switch (Content)
        {
            case ImageTint img:
                img.TintColor = color;
                break;
            case Label label:
                label.TextColor = color;
                break;
            default:
                break;
        }
    }
}
