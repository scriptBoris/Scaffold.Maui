using Scaffold.Maui.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scaffold.Maui;

public class MenuItem : BindableObject
{
    private MenuItemObs? parent;

    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MenuItem),
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
        typeof(MenuItem),
        null
    );
    public ImageSource? ImageSource
    {
        get => GetValue(ImageSourceProperty) as ImageSource;
        set => SetValue(ImageSourceProperty, value);
    }

    // image color
    public static readonly BindableProperty ImageColorProperty = BindableProperty.Create(
        nameof(ImageColor),
        typeof(Color),
        typeof(MenuItem),
        null
    );
    public Color? ImageColor
    {
        get => GetValue(ImageColorProperty) as Color;
        set => SetValue(ImageColorProperty, value);
    }

    // command
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command),
        typeof(ICommand),
        typeof(MenuItem),
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
        typeof(MenuItem),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItem self)
                self.Update();
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
        typeof(MenuItem),
        true,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItem self)
                self.Update();
        }
    );
    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    internal void SetupParent(MenuItemObs? parent)
    {
        this.parent = parent;
    }

    private void Update()
    {
        if (parent == null)
            return;

        parent.Update();
    }
}

// todo delete?
internal class MenuItemProxy : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
}