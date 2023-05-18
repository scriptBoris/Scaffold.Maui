using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ScaffoldLib.Maui.Internal;

internal partial class ImageTintHandler
{
    private bool hasHandler;
    private ImageTint Proxy => (ImageTint)VirtualView;

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        SetTint(Proxy.TintColor);
    }

    public void SetTint(Microsoft.Maui.Graphics.Color? color)
    {
        if (!hasHandler)
            return;

        if (PlatformView.Image == null)
            return;

        if (color != null)
        {
            PlatformView.Image = PlatformView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            PlatformView.TintColor = color.ToPlatform();
        }
        else
        {
            PlatformView.Image = PlatformView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
        }
    }

    protected override void ConnectHandler(UIImageView platformView)
    {
        hasHandler = true;
        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(UIImageView platformView)
    {
        hasHandler = false;
        base.DisconnectHandler(platformView);
    }
}
