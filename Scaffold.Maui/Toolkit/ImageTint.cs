using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit;

public partial class ImageTintHandler : ImageHandler
{
}

public class ImageTint : Image, IDisposable
{
    public ImageTint()
    {
        InputTransparent = true;    
    }

    // tint color
    public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
        nameof(TintColor),
        typeof(Color),
        typeof(ImageTint),
        null
    );
    public Color? TintColor
    {
        get => GetValue(TintColorProperty) as Color;
        set => SetValue(TintColorProperty, value);
    }

    public void Dispose()
    {
        Handler = null;
    }
}
