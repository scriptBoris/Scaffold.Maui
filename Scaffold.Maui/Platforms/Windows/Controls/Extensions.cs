using Microsoft.Maui.Handlers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Windows.Controls;

internal static class Extensions
{
    public static Microsoft.UI.Windowing.AppWindow ToAppWindow(this Microsoft.Maui.Controls.Window window)
    {
        var h = window.Handler as WindowHandler;
        var w = h.PlatformView;
        return w.ToAppWindow();
    }

    public static Microsoft.UI.Windowing.AppWindow ToAppWindow(this Microsoft.UI.Xaml.Window window)
    {
        var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
        var appwindow = AppWindow.GetFromWindowId(windowId);
        return appwindow;
    }
}
