using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ScaffoldLib.Maui.Platforms.iOS
{
    internal class PlatformSpecific : IPlatformSpecific
    {
        public Thickness GetSafeArea()
        {
            double top = 0;
            double bottom = 0;
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var window = UIApplication.SharedApplication.Windows.LastOrDefault();
                if (window != null)
                {
                    top = window.SafeAreaInsets.Top;
                    bottom = window.SafeAreaInsets.Bottom;
                }
            }
            else
            {
                top = UIApplication.SharedApplication.StatusBarFrame.Height;
            }

            return new Thickness(0, top, 0, bottom);
        }
    }
}
