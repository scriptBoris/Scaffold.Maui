using Android.App;
using Android.OS;
using Android.Views;
using Microsoft.Maui.LifecycleEvents;
using MView = Microsoft.Maui.Controls.View;

namespace ScaffoldLib.Maui.Platforms.Android
{
    public static class ScaffoldAndroid
    {
        internal static void Init(MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(x =>
            {
                x.AddAndroid(a =>
                {
                    a.OnBackPressed(OnBackPressed);
                    a.OnCreate(OnCreate);
                });
            });
        }

        private static bool OnBackPressed(Activity a)
        {
            Scaffold? rootScaffold = null;
            var mp = global::Microsoft.Maui.Controls.Application.Current?.MainPage;
            if (mp is ContentPage c)
            {
                if (c.Content is Scaffold cv)
                    rootScaffold = cv;
                else
                    rootScaffold = Find(c.Content) as Scaffold;
            }

            if (rootScaffold != null)
            {
                rootScaffold.Dispatcher.Dispatch(async () =>
                {
                    var nested = rootScaffold
                        .GetScafoldNested()
                        .Reverse()
                        .ToArray();

                    foreach (var item in nested)
                    {
                        if (item is not Scaffold scaffold)
                            continue;

                        if (scaffold.ZBuffer.Pop())
                            return;
                    }

                    foreach (var item in nested)
                    {
                        if (item is not Scaffold scaffold)
                            continue;

                        if (scaffold.BackButtonBehavior?.OverrideHardwareBackButtonAction(item) == true)
                            return;

                        if (await scaffold.HardwareBackButtonInternal())
                            return;
                    }

                    a.MoveTaskToBack(true);
                });
                return true;
            }

            return false;
        }

        private static void OnCreate(Activity a, Bundle? savedInstanceState)
        {
            // setup transparent statusbar
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                a.Window!.SetStatusBarColor(global::Android.Graphics.Color.Transparent);
                var flag = SystemUiFlags.LayoutFullscreen | SystemUiFlags.LayoutStable;
                var root = a.FindViewById(global::Android.Resource.Id.Content)!;

                // todo разобраться как правильно и безопасно делать statusbar прозрачным и что бы можно было разместить контент под ним
                root.SystemUiVisibility = (StatusBarVisibility)flag;
            }
        }

        private static MView? Find(MView view)
        {
            switch (view)
            {
                case Scaffold vc:
                    return vc;

                case ContentView cv:
                    return Find(cv);

                case Layout l:
                    foreach (var item in l.Children)
                        return Find((MView)item);
                    break;

                default:
                    return null;
            }

            return null;
        }

        internal static Thickness GetSafeArea()
        {
            double statusBarHeight = 0;
            var context = global::Android.App.Application.Context;

            int resourceId = context.Resources?.GetIdentifier("status_bar_height", "dimen", "android") ?? 0;
            if (resourceId > 0)
            {
                int h = context.Resources?.GetDimensionPixelSize(resourceId) ?? 0;
                double den = Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
                statusBarHeight = (h != 0) ? h / den : 0;
            }

            return new Thickness(0, statusBarHeight, 0, 0);
        }
    }
}