using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Windows;

internal static class ScaffoldWindows
{
    internal static void Init(MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(x =>
        {
            x.AddWindows(x =>
            {
                x.OnActivated((w, e) =>
                {
                });

                x.OnWindowCreated(w =>
                {
                    if (w is not MauiWinUIWindow window)
                        return;

                    window.ExtendsContentIntoTitleBar = false;
                    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                    switch (appWindow.Presenter)
                    {
                        case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                            overlappedPresenter.IsMaximizable = false;
                            overlappedPresenter.IsResizable = false;
                            overlappedPresenter.IsMaximizable = false;
                            overlappedPresenter.SetBorderAndTitleBar(false, false);
                            break;
                    }
                });
            });
        });
    }
}
