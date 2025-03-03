using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.StaticLibs.ButtonSam.Core;
using UIKit;

namespace ScaffoldLib.Maui.Platforms.iOS.ButtonSam;

public class ButtonHandler : Microsoft.Maui.Handlers.LayoutHandler, IButtonHandler
{
    public ButtonHandler() : base(PropertyMapper)
    {
    }

    public static readonly PropertyMapper<InteractiveContainer, ButtonHandler> PropertyMapper = new(ViewMapper)
    {
        [nameof(InteractiveContainer.BackgroundColor)] = (h, v) =>
        {
            var color = v.BackgroundColor ?? InteractiveContainer.DefaultBackgroundColor;
            h.DirectSetBackgroundColor(color);
        },
        [nameof(InteractiveContainer.BorderWidth)] = (h, v) =>
        {
            if (h.Native != null)
                h.Native.BorderWidth = v.BorderWidth;
        },
        [nameof(InteractiveContainer.BorderColor)] = (h, v) =>
        {
            if (h.Native != null)
                h.Native.BorderColor = v.BorderColor?.ToPlatform();
        },
        [nameof(InteractiveContainer.CornerRadius)] = (h, v) =>
        {
            if (h.Native != null)
                h.Native.CornerRadius = v.CornerRadius;
        },
        [nameof(InteractiveContainer.IsClickable)] = (h, v) =>
        {
            if (h.Native != null)
            {
                h.Native.IsClickable = v.IsClickable;
            }
        },
    };

    public InteractiveContainer Proxy => (InteractiveContainer)VirtualView;
    public ButtonIos? Native => PlatformView as ButtonIos;

    protected override LayoutView CreatePlatformView()
    {
        var native = new ButtonIos(this);
        return native;
    }

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        var color = Proxy.BackgroundColor ?? InteractiveContainer.DefaultBackgroundColor;
        DirectSetBackgroundColor(color);
    }

    public virtual void DirectSetBackgroundColor(Color color)
    {
        Native?.SetupBackground(color.ToPlatform());
    }
}