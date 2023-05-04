using Android.App;
using Microsoft.Maui.LifecycleEvents;

namespace Scaffold.Maui.Platforms.Android
{
    public static class Initializer
    {
        internal static void Init(MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(x => {
                x.AddAndroid(a =>
                {
                    a.OnBackPressed(OnBackPressed);
                });
            });
        }

        private static bool OnBackPressed(Activity a)
        {
            ScaffoldView? match = null;
            var mp = global::Microsoft.Maui.Controls.Application.Current?.MainPage;
            if (mp is ContentPage c)
            {
                if (c.Content is ScaffoldView cv)
                    match = cv;
                else
                    match = Find(c.Content) as ScaffoldView;
            }

            match?.PopAsync(true);

            return true;
        }

        private static View? Find(View view)
        {
            switch (view)
            {
                case ScaffoldView vc:
                    return vc;

                case ContentView cv:
                    return Find(cv);

                case Layout l:
                    foreach (var item in l.Children)
                        return Find((View)item);
                    break;

                default: 
                    return null;
            }

            return null;
        }
    }
}