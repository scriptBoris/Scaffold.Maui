using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Windows.Win32Utils;

public static class Windows32
{
    // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
    // Copied from dwmapi.h
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
    // what value of the enum to set.
    // Copied from dwmapi.h
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    public enum ShowWindowOptions
    {
        FORCEMINIMIZE = 11,
        HIDE = 0,
        MAXIMIZE = 3,
        MINIMIZE = 6,
        RESTORE = 9,
        SHOW = 5,
        SHOWDEFAULT = 10,
        SHOWMAXIMIZED = 3,
        SHOWMINIMIZED = 2,
        SHOWMINNOACTIVE = 7,
        SHOWNA = 8,
        SHOWNOACTIVATE = 4,
        SHOWNORMAL = 1
    }

    // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    private static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

    [DllImport("user32.dll")]
    private static extern int ShowWindow(IntPtr hwnd, ShowWindowOptions cmdShow);

    public static void SetupCornersStyle(this MauiWinUIWindow window, bool useRoundedCorners)
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
        var preference = useRoundedCorners ? DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND : DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
        DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
    }

    public static void MaximizeAsWin32(this MauiWinUIWindow window)
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        ShowWindow(hWnd, ShowWindowOptions.SHOWMAXIMIZED);
    }
}