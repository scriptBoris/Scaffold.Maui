using ButtonSam.Maui;
using Microsoft.Maui.LifecycleEvents;
using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "ScaffoldLib.Maui")]

namespace ScaffoldLib.Maui;

public static class Initializer
{
    internal static bool IsInitialized { get; private set; }
    internal static bool UseDebugInfo { get; private set; }

    public static MauiAppBuilder UseScaffold(this MauiAppBuilder builder, UseScaffoldArgs? configArgs = null)
    {
        configArgs ??= new();

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

        builder.UseButtonSam(configArgs.UseDebugInfo);

        IsInitialized = true;
        return builder;
    }
}
