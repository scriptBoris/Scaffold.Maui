using ButtonSam.Maui;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "ScaffoldLib.Maui")]

namespace ScaffoldLib.Maui;

public static class Initializer
{
    internal static bool IsInitialized;

    public static MauiAppBuilder UseScaffold(this MauiAppBuilder builder)
    {
#if ANDROID
        Platforms.Android.ScaffoldAndroid.Init(builder);
#elif IOS
        Platforms.iOS.ScaffoldIOS.Init(builder);
#elif WINDOWS
        Platforms.Windows.ScaffoldWindows.Init(builder);
#endif

        Scaffold.Preserve();

        builder.ConfigureMauiHandlers(h =>
        {
#if ANDROID
            h.AddHandler(typeof(ImageTint), typeof(Platforms.Android.ImageTintHandler));
#elif IOS
            h.AddHandler(typeof(ImageTint), typeof(Platforms.iOS.ImageTintHandler));
#elif WINDOWS
            h.AddHandler(typeof(ImageTint), typeof(Platforms.Windows.ImageTintHandler));
#endif
        });

        builder.UseButtonSam();

        IsInitialized = true;
        return builder;
    }
}
