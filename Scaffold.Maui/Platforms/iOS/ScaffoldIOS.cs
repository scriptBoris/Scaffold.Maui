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

        builder.ConfigureMauiHandlers(x =>
        {
            x.AddHandler(typeof(GlassView), typeof(GlassHandler));
        });

        Microsoft.Maui.Handlers.LayoutHandler.ElementMapper.Add(nameof(View.Handler), (h, e) =>
        {
            if (e is Layout layout)
            {
                layout.IgnoreSafeArea = true;
            }
        });
    }

    private static bool OnFinishedLaunching(UIApplication a, NSDictionary e)
    {
        UpdateSafeArea();

        var page = Microsoft.Maui.Controls.Application.Current?.MainPage;
        if (page != null)
        {
            page.SizeChanged += (o, e) =>
            {
                UpdateSafeArea();
            };
        }

        return true;
    }

    private static void UpdateSafeArea()
    {
        var safe = Scaffold.PlatformSpec.GetSafeArea();
        Scaffold.SafeArea = safe;
        var page = Microsoft.Maui.Controls.Application.Current?.MainPage;
        if (page != null)
            page.OnThisPlatform().SetUseSafeArea(false);
    }
}
