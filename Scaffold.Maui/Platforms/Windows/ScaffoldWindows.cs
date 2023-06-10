using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Platforms.Windows.Controls;
using Windows.UI.WindowManagement;

namespace ScaffoldLib.Maui.Platforms.Windows;

internal static class ScaffoldWindows
{
    internal static StatusBarColorTypes InitialColorScheme = StatusBarColorTypes.Dark;
    internal static WindowController? WindowController;

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

                    Scaffold.SafeArea = new Microsoft.Maui.Thickness(0, 40, 0, 0);
                    var rootScaffold = Application.Current.MainPage.GetRootScaffold();

                    window.ExtendsContentIntoTitleBar = false;
                    var appWindow = window.ToAppWindow();
                    var titleBar = appWindow.TitleBar;
                    titleBar.ExtendsContentIntoTitleBar = true;

                    switch (appWindow.Presenter)
                    {
                        case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                            overlappedPresenter.IsMaximizable = false;
                            overlappedPresenter.IsResizable = false;
                            overlappedPresenter.IsMaximizable = false;
                            overlappedPresenter.SetBorderAndTitleBar(true, false);
                            break;
                    }

                    WindowController = new WindowController(window, rootScaffold, InitialColorScheme);
                });
            });
        });
    }
}
