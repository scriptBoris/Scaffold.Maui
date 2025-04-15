using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ScaffoldLib.Maui.Platforms.iOS;

internal class LabelNativeHandler : LabelHandler
{
    internal static readonly PropertyMapper<LabelNative, LabelNativeHandler> _mapper = new(Mapper)
    {
        [nameof(LabelNative.StyleAttribute)] = MapAttribute,
        [nameof(ITextStyle.Font)] = MapAttribute,
    };

    private static void MapAttribute(LabelNativeHandler handler, LabelNative native)
    {
        if (native.FontFamily != null)
        {
            var fontManager = handler.MauiContext.Services.GetRequiredService<IFontManager>();
            handler.PlatformView?.UpdateFont(native, fontManager);
        }
        else
        {
            switch (native.StyleAttribute)
            {
                case LabelNativeAttributes.NavigationTitle:
                case LabelNativeAttributes.AlertTitle:
                    var font = UIFont.SystemFontOfSize((nfloat)native.FontSize, UIFontWeight.Semibold);
                    handler.PlatformView.Font = font;
                    break;
                case LabelNativeAttributes.None:
                case LabelNativeAttributes.AlertDescription:
                case LabelNativeAttributes.AlertButton:
                default:
                    var fontdef = UIFont.SystemFontOfSize((nfloat)native.FontSize);
                    handler.PlatformView.Font = fontdef;
                    break;
            }
        }
    }

    public LabelNativeHandler() : base(_mapper)
    {
    }
}