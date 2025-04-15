using Android.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Android;

internal class LabelNativeHandler : LabelHandler
{
    internal static PropertyMapper<LabelNative, LabelNativeHandler> _mapper = new(Mapper)
    {
        [nameof(LabelNative.StyleAttribute)] = MapStyleAttribute,
        [nameof(ITextStyle.Font)] = MapStyleAttribute,
    };

    public LabelNativeHandler() : base(_mapper)
    {
    }

    private static void MapStyleAttribute(LabelNativeHandler handler, LabelNative native)
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
                    var font = Typeface.Create("sans-serif-medium", TypefaceStyle.Normal);
                    handler.PlatformView.Typeface = font;
                    break;
                case LabelNativeAttributes.None:
                case LabelNativeAttributes.AlertDescription:
                case LabelNativeAttributes.AlertButton:
                default:
                    handler.PlatformView.Typeface = Typeface.Default;
                    break;
            }

            handler.PlatformView.SetTextSize(global::Android.Util.ComplexUnitType.Sp, (float)native.FontSize);
        }
    }
}