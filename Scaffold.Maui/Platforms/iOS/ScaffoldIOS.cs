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

namespace ScaffoldLib.Maui.Platforms.iOS
{
    internal static class ScaffoldIOS
    {
        internal static void Init(MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(x =>
            {
                x.AddiOS(ios =>
                {
                    ios.FinishedLaunching((a, e) =>
                    {
                        var page = Microsoft.Maui.Controls.Application.Current?.MainPage;
                        //var window = page?.GetParentWindow();

                        if (page != null)
                        {
                            var safe = new PlatformSpecific().GetSafeArea();
                            page.OnThisPlatform().SetUseSafeArea(false);
                            page.Padding = new Thickness(0, -safe.Top, 0, -safe.Bottom);

                            var scaffold = page.GetRootScaffold();
                            if (scaffold != null)
                                scaffold.SafeArea = safe;
                        }

                        //if (page.Handler is PageHandler p)
                        //{
                        //    var container = p.PlatformView;
                        //    container.InsetsLayoutMarginsFromSafeArea = false;
                        //    var mm = container.LayoutMargins;
                        //}

                        return true;
                    });
                });
            });
        }
    }
}
