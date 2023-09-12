using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Windows
{
    internal class PlatformSpecific : IPlatformSpecific
    {
        public Thickness GetSafeArea()
        {
            return new Thickness(0, 40, 0, 0);
        }

        public void SetStatusBarColorScheme(StatusBarColorTypes scheme)
        {
            ScaffoldWindows.ResolveStatusBarScheme(scheme);
        }

        public void UpdateDesktopDragArea()
        {
            ScaffoldWindows.UpdateWindowDragArea();
        }
    }
}
