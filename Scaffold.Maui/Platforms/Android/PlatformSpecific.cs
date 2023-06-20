using Android.App;
using Android.OS;
using AndroidX.Core.View;
using Google.Android.Material.Elevation;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Android
{
    internal class PlatformSpecific : IPlatformSpecific
    {
        public Thickness GetSafeArea()
        {
            double statusBarHeight = GetStatusBarHeight();
            return new Thickness(0, statusBarHeight, 0, 0);
        }

        public async void SetStatusBarColorScheme(StatusBarColorTypes colorType)
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            if (activity == null)
                activity = await Platforms.Android.ScaffoldAndroid.AwaitActivity.Task;

            if (activity?.Window == null)
                return;
            
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                int i1;
                int i2;
                switch (colorType)
                {
                    case StatusBarColorTypes.Light:
                        i1 = (int)global::Android.Views.WindowInsetsControllerAppearance.None;
                        i2 = (int)global::Android.Views.WindowInsetsControllerAppearance.LightStatusBars;
                        break;
                    case StatusBarColorTypes.Dark:
                        i1 = (int)global::Android.Views.WindowInsetsControllerAppearance.LightStatusBars;
                        i2 = (int)global::Android.Views.WindowInsetsControllerAppearance.LightStatusBars;
                        break;
                    default:
                        throw new ArgumentException($"value {colorType} is not supported.");
                }

                activity.Window.InsetsController?.SetSystemBarsAppearance(i1, i2);
            }
            else
            {
                var ctrl = ViewCompat.GetWindowInsetsController(activity.Window.DecorView);
                ctrl.AppearanceLightStatusBars = colorType == StatusBarColorTypes.Dark;
            }
        }

        private static double GetStatusBarHeight()
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
            return statusBarHeight;
        }
    }
}
