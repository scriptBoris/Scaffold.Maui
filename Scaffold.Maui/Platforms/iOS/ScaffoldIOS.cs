using Foundation;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ScaffoldLib.Maui.Platforms.iOS;

internal static class ScaffoldIOS
{
    internal static void Init(MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(x =>
        {
            x.AddiOS(ios =>
            {
                ios.FinishedLaunching(OnFinishedLaunching);
                ios.OnActivated(e =>
                {
                    var scaffold = Microsoft.Maui.Controls.Application.Current?.MainPage?.GetRootScaffold();
                    if (scaffold != null)
                    {
                        scaffold.OnAppear(false);
                        scaffold.OnAppear(true);
                    }
                });
                ios.DidEnterBackground(e =>
                {
                    var scaffold = Microsoft.Maui.Controls.Application.Current?.MainPage?.GetRootScaffold();
                    if (scaffold != null)
                    {
                        scaffold.OnDisappear(false);
                        scaffold.OnDisappear(true);
                    }
                });
            });
        });
    }

    private static bool OnFinishedLaunching(UIApplication a, NSDictionary e)
    {
        var safe = Scaffold.PlatformSpec.GetSafeArea();
        Scaffold.SafeArea = safe;

        var page = Microsoft.Maui.Controls.Application.Current?.MainPage;
        if (page != null)
        {
            page.SizeChanged += (o, e) =>
            {
                UpdateSafeArea();
            };
            page.OnThisPlatform().SetUseSafeArea(false);
            page.Padding = new Thickness(0, -safe.Top, 0, -safe.Bottom);
        }

        return true;
    }

    private static void UpdateSafeArea()
    {
        var page = Microsoft.Maui.Controls.Application.Current?.MainPage;
        if (page != null)
        {
            var safe = Scaffold.PlatformSpec.GetSafeArea();
            Scaffold.SafeArea = safe;
            page.OnThisPlatform().SetUseSafeArea(false);
            page.Padding = new Thickness(-safe.Left, -safe.Top, -safe.Right, -safe.Bottom);
        }
    }
}
