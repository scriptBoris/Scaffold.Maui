using ButtonSam.Maui;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui;

public static class Initializer
{
    internal static bool IsInitialized;

    public static MauiAppBuilder UseScaffold(this MauiAppBuilder builder)
    {
#if ANDROID
        Platforms.Android.ScaffoldAndroid.Init(builder);
#elif WINDOWS
        Platforms.Windows.ScaffoldWindows.Init(builder);
#elif IOS
        Platforms.iOS.ScaffoldIOS.Init(builder);
#endif

        builder.ConfigureMauiHandlers(h =>
        {
            h.AddHandler(typeof(ImageTint), typeof(ImageTintHandler));
        });

        builder.UseButtonSam();

        IsInitialized = true;
        return builder;
    }
}
