using Android.App;
using Android.OS;
using Android.Views;
using Microsoft.Maui.LifecycleEvents;
using ScaffoldLib.Maui.Internal;
using MView = Microsoft.Maui.Controls.View;

namespace ScaffoldLib.Maui.Platforms.Android;

public static class ScaffoldAndroid
{
    internal static TaskCompletionSource<Activity> AwaitActivity { get; private set; } = new();

    internal static void Init(MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(x =>
        {
            x.AddAndroid(a =>
            {
                a.OnCreate(OnCreate);
                a.OnBackPressed(OnBackPressed);
                a.OnStart(a =>
                {
                    var scaffold = Microsoft.Maui.Controls.Application.Current?.MainPage?.GetRootScaffold();
                    if (scaffold != null)
                    {
                        scaffold.OnAppear(false);
                        scaffold.OnAppear(true);
                    }
                });
                a.OnStop(a =>
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

    private static void OnCreate(Activity a, Bundle? savedInstanceState)
    {
        // setup transparent statusbar
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
        {
            a.Window!.SetStatusBarColor(global::Android.Graphics.Color.Transparent);
            var flag = SystemUiFlags.LayoutFullscreen | SystemUiFlags.LayoutStable;
            var root = a.FindViewById(global::Android.Resource.Id.Content)!;

            // todo разобраться как правильно и безопасно делать statusbar прозрачным и чтобы
            // можно было разместить контент под ним
            root.SystemUiVisibility = (StatusBarVisibility)flag;
        }
        AwaitActivity.TrySetResult(a);

        var safe = Scaffold.PlatformSpec.GetSafeArea();
        Scaffold.SafeArea = safe;

        var scaffold = Microsoft.Maui.Controls.Application.Current?.MainPage?.GetRootScaffold();
        if (scaffold != null)
        {
            scaffold.OnAppear(false);
            scaffold.OnAppear(true);
        }
    }

    private static bool OnBackPressed(Activity a)
    {
        var rootScaffold = Microsoft.Maui.Controls.Application.Current?.MainPage?.GetRootScaffold();
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
}