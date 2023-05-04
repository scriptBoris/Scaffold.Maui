using Android.Graphics;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Internal
{
    public partial class ImageTintHandler
    {
        private ImageTint Proxy => (ImageTint)VirtualView;

        public override void SetVirtualView(IView view)
        {
            base.SetVirtualView(view);
            SetTint(Proxy.TintColor);
        }

        public void SetTint(Microsoft.Maui.Graphics.Color? color)
        {
            if (color != null)
            {
                var src = PorterDuff.Mode.SrcIn ?? throw new InvalidOperationException("PorterDuff.Mode.SrcIn should not be null at runtime.");
                var port = new PorterDuffColorFilter(color.ToPlatform(), src);
                PlatformView.SetColorFilter(port);
            }
            else
            {
                PlatformView.SetColorFilter(null);
            }
        }
    }
}
