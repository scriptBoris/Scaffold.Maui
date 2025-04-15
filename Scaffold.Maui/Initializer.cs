using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.StaticLibs.ButtonSam;
using ScaffoldLib.Maui.Toolkit;
using System.Diagnostics;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "ScaffoldLib.Maui")]
namespace ScaffoldLib.Maui;

public static class Initializer
{
    private static bool? _isDebugMode;

    internal static bool IsInitialized { get; private set; }
    internal static bool UseDebugInfo { get; private set; }
    internal static bool IsDebugMode
    {
        get
        {
            _isDebugMode ??= !DetectReleaseMode();
            return _isDebugMode.Value;
        }
    }

    public static MauiAppBuilder UseScaffold(this MauiAppBuilder builder, UseScaffoldArgs? configArgs = null)
    {
        configArgs ??= new();
        UseDebugInfo = configArgs.UseDebugInfo;

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
            h.AddHandler(typeof(Toolkit.RadioButton), typeof(Platforms.Android.ToolkitRadioButtonHandler));
            h.AddHandler(typeof(LabelNative), typeof(Platforms.Android.LabelNativeHandler));
#elif IOS
            h.AddHandler(typeof(ImageTint), typeof(Platforms.iOS.ImageTintHandler));
            h.AddHandler(typeof(LabelNative), typeof(Platforms.iOS.LabelNativeHandler));
#elif WINDOWS
            h.AddHandler(typeof(ImageTint), typeof(Platforms.Windows.ImageTintHandler));
            h.AddHandler(typeof(LabelNative), typeof(Platforms.Windows.LabelNativeHandler));
#endif
        });

        builder.UseButtonSam();

        IsInitialized = true;
        return builder;
    }

    private static bool DetectReleaseMode()
    {
        var assembly = Application.Current.GetType().Assembly;
        object[] attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), true);
        if (attributes == null || attributes.Length == 0)
            return true;

        var d = (DebuggableAttribute)attributes[0];
        if ((d.DebuggingFlags & DebuggableAttribute.DebuggingModes.Default) == DebuggableAttribute.DebuggingModes.None)
            return true;

        return false;
    }
}