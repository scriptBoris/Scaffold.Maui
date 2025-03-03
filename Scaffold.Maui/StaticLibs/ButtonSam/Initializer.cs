using ScaffoldLib.Maui.StaticLibs.ButtonSam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.StaticLibs.ButtonSam;

internal static class Initializer
{
    internal static bool UseDebugInfo { get; private set; }

    internal static MauiAppBuilder UseButtonSam(this MauiAppBuilder builder, bool useDebugOutputInfo = false)
    {
        UseDebugInfo = useDebugOutputInfo;

        builder.ConfigureMauiHandlers(x =>
        {
#if ANDROID
            x.AddHandler(typeof(InteractiveContainer), typeof(ScaffoldLib.Maui.Platforms.Android.ButtonSam.ButtonHandler));
#elif IOS
            x.AddHandler(typeof(InteractiveContainer), typeof(ScaffoldLib.Maui.Platforms.iOS.ButtonSam.ButtonHandler));
#elif WINDOWS
            x.AddHandler(typeof(InteractiveContainer), typeof(ScaffoldLib.Maui.Platforms.Windows.ButtonSam.ButtonHandler));
#else
            throw new NotImplementedException();
#endif
        });

        return builder;
    }
}
