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
