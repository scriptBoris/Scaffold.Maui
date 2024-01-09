using Android.Content;
using Android.Graphics;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScaffoldLib.Maui.Toolkit;

namespace ScaffoldLib.Maui.Platforms.Android;

public class ImageTintHandler : ImageHandler
{
    public ImageTintHandler() : base(ImageTintHandlerMapper)
    {
    }

    public static PropertyMapper<ImageTint, ImageTintHandler> ImageTintHandlerMapper = new(Mapper)
    {
        [nameof(ImageTint.TintColor)] = MapTintColor,
    };

    public static void MapTintColor(ImageTintHandler h, ImageTint v)
    {
        var color = (h.VirtualView as ImageTint)?.TintColor;
        if (color != null)
        {
            var src = PorterDuff.Mode.SrcIn ?? throw new InvalidOperationException("PorterDuff.Mode.SrcIn should not be null at runtime.");
            var port = new PorterDuffColorFilter(color.ToPlatform(), src);
            h.PlatformView?.SetColorFilter(port);
        }
        else
        {
            h.PlatformView?.SetColorFilter(null);
        }
    }
}
