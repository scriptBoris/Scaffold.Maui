using ButtonSam.Maui;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui;

public static class Initializer
{
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
        return builder;
    }
}
