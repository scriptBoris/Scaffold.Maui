using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Platforms.Windows.Controls;

namespace ScaffoldLib.Maui.Platforms.Windows;

internal static class ScaffoldWindows
{
    private static StatusBarColorTypes initialColorScheme = StatusBarColorTypes.Dark;
    private static Microsoft.UI.Windowing.AppWindowTitleBar? titleBar;
    private static Scaffold? rootScaffold;
    private static int undragCache;

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

                    Scaffold.DeviceSafeArea = new Microsoft.Maui.Thickness(0, 40, 0, 0);
                    rootScaffold = Application.Current?.MainPage?.GetRootScaffold();
                    if (rootScaffold == null)
                        return;

                    window.ExtendsContentIntoTitleBar = false;
                    var appWindow = window.ToAppWindow();
                    titleBar = appWindow.TitleBar;
                    titleBar.ExtendsContentIntoTitleBar = true;
                    titleBar.ButtonBackgroundColor = Colors.Transparent.ToWindowsColor();
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent.ToWindowsColor();
                    titleBar.ButtonInactiveForegroundColor = Colors.Gray.ToWindowsColor();
                    ResolveStatusBarScheme(initialColorScheme);
                });
            });
        });
    }

    internal static void ResolveStatusBarScheme(StatusBarColorTypes scheme)
    {
        if (titleBar == null)
        {
            initialColorScheme = scheme;
            return;
        }

        switch (scheme)
        {
            case StatusBarColorTypes.Light:
                titleBar.ButtonForegroundColor = Colors.White.ToWindowsColor();
                break;
            case StatusBarColorTypes.Dark:
                titleBar.ButtonForegroundColor = Colors.Black.ToWindowsColor();
                break;
            default:
                break;
        }
    }

    internal static void UpdateWindowDragArea()
    {
        if (rootScaffold == null || titleBar == null)
            return;

        var dragRect = new Rect(0, 0, rootScaffold.Width, 40);
        var undragRects = rootScaffold.UndragArea;
        int hash = HashCode.Combine(dragRect.GetHashCode(), undragRects.CalcHash());
        if (hash == undragCache)
            return;

        var undrags = Inverse(undragRects, dragRect);
        var rects = new List<global::Windows.Graphics.RectInt32>();
        foreach (var item in undrags)
            rects.Add(new global::Windows.Graphics.RectInt32(
                (int)item.X,
                (int)item.Y,
                (int)item.Width,
                (int)item.Height)
            );

        titleBar.SetDragRectangles(rects.ToArray());
        undragCache = hash;
    }

    private static Rect[] Inverse(Rect[] interactiveRects, Rect whereArea)
    {
        var nonInteractiveRects = new List<Rect>
        {
            whereArea
        };

        foreach (Rect interactiveRect in interactiveRects)
        {
            var newNonInteractiveRects = new List<Rect>();

            foreach (Rect nonInteractiveRect in nonInteractiveRects)
            {
                if (nonInteractiveRect.IntersectsWith(interactiveRect))
                    newNonInteractiveRects.AddRange(SubtractRect(nonInteractiveRect, interactiveRect));
                else
                    newNonInteractiveRects.Add(nonInteractiveRect);
            }

            nonInteractiveRects = newNonInteractiveRects;
        }

        return nonInteractiveRects.ToArray();
    }

    private static List<Rect> SubtractRect(Rect originalRect, Rect subtractRect)
    {
        var fragments = new List<Rect>();

        if (originalRect.Left < subtractRect.Left)
            fragments.Add(new Rect(originalRect.Left, originalRect.Top, subtractRect.Left - originalRect.Left, originalRect.Height));

        if (originalRect.Right > subtractRect.Right)
            fragments.Add(new Rect(subtractRect.Right, originalRect.Top, originalRect.Right - subtractRect.Right, originalRect.Height));

        if (originalRect.Top < subtractRect.Top)
            fragments.Add(new Rect(originalRect.Left, originalRect.Top, originalRect.Width, subtractRect.Top - originalRect.Top));

        if (originalRect.Bottom > subtractRect.Bottom)
            fragments.Add(new Rect(originalRect.Left, subtractRect.Bottom, originalRect.Width, originalRect.Bottom - subtractRect.Bottom));

        return fragments;
    }

    private static int CalcHash(this Rect[] self)
    {
        int last = 0;
        foreach (object item in self)
            last = HashCode.Combine(last, item.GetHashCode());

        return last;
    }
}
