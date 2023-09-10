using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

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
        null,
        propertyChanged: (b, o, n) =>
        {
#if WINDOWS
            if (b is ImageTint self) self.UpdateSource();
#else
            if (b is ImageTint self && self.Handler is ImageTintHandler h) h.SetTint(n as Color);
#endif
        }
    );
    public Color? TintColor
    {
        get => GetValue(TintColorProperty) as Color;
        set => SetValue(TintColorProperty, value);
    }

    public static new readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(ImageSource),
        typeof(ImageTint),
        null,
        propertyChanged: (b, o, n) =>
        {
#if WINDOWS
            if (b is ImageTint self) self.UpdateSource();
#else
            if (b is Image self) self.Source = n as ImageSource;
#endif
        }
    );
    public new ImageSource? Source
    {
        get => GetValue(SourceProperty) as ImageSource;
        set => SetValue(SourceProperty, value);
    }

    public void Dispose()
    {
        Handler = null;
    }

#if WINDOWS
    private CancellationToken cancel;
    private async void UpdateSource()
    {
        cancel.ThrowIfCancellationRequested();
        cancel = new CancellationToken();
        var res = await Platforms.Windows.WinImageTools.ProcessImage(Source, TintColor, cancel);
        if (res.IsCanceled)
            return;

        base.Source = res.Source;
    }
#endif
}
