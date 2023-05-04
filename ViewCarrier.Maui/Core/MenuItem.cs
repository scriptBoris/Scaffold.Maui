using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scaffold.Maui;

public class MenuItem : BindableObject
{
    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MenuItem),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItem self)
                self.Update();
        }
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
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItem self)
                self.Update();
        }
    );
    public ICommand? Command
    {
        get => GetValue(CommandProperty) as ICommand;
        set => SetValue(CommandProperty, value);
    }

    private void Update()
    {

    }
}
