using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using AView = Android.Views.View;
#endif

namespace ScaffoldLib.Maui.Toolkit
{
    public static class Extensions
    {
        public static string GetDisplayItemText(this object self, string? displayProperty)
        {
            object? currentObject = self;
            if (!string.IsNullOrWhiteSpace(displayProperty))
            {
                string[] propertyNames = displayProperty.Split('.');

                foreach (string propertyName in propertyNames)
                {
                    var propertyInfo = currentObject.GetType().GetProperty(propertyName);
                    if (propertyInfo == null)
                    {
                        currentObject = null;
                        break;
                    }

                    currentObject = propertyInfo.GetValue(currentObject);
                    if (currentObject == null)
                    {
                        currentObject = null;
                        break;
                    }
                }

                if (currentObject == null)
                    currentObject = self;
            }

            return currentObject.ToString() ?? self.GetType().Name;
        }

        public static Point? GetPositionOnDisplay(this View view)
        {
#if ANDROID
            int[] location = new int[2];
            var native = view.Handler?.PlatformView as AView;
            if (native == null)
                return null;

            native.GetLocationOnScreen(location);
            int x = location[0];
            int y = location[1];
            double den = Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo.Density;
            double xres = x / den;
            double yres = y / den;
            return new Point(xres, yres);
#else
            //var window = App.Current.Windows.First<Window>().Handler.PlatformView;
            //var platformview = CounterBtn.Handler.PlatformView as Microsoft.Maui.Platform.MauiButton;
            //var point = platformview.TransformToVisual(window.Content).TransformPoint(new Windows.Foundation.Point(0, 0));
            throw new NotImplementedException();
#endif


        }
    }
}
