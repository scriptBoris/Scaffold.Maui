using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Windows;

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
            handler.PlatformView.FontSize = native.FontSize;
            switch (native.StyleAttribute)
            {
                case LabelNativeAttributes.NavigationTitle:
                    handler.PlatformView.FontWeight = global::Microsoft.UI.Text.FontWeights.Normal;
                    break;
                case LabelNativeAttributes.AlertTitle:
                    handler.PlatformView.FontWeight = global::Microsoft.UI.Text.FontWeights.Medium;
                    break;
                case LabelNativeAttributes.None:
                case LabelNativeAttributes.AlertDescription:
                case LabelNativeAttributes.AlertButton:
                default:
                    handler.PlatformView.FontWeight = global::Microsoft.UI.Text.FontWeights.Normal;
                    break;
            }
        }
    }
}