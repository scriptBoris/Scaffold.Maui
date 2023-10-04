using Android.Content;
using Android.Graphics;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit
{
    public partial class ImageTintHandler
    {
        private bool hasHandler;
        private ImageTint Proxy => (ImageTint)VirtualView;

        public override void SetVirtualView(IView view)
        {
            base.SetVirtualView(view);
            SetTint(Proxy.TintColor);
        }

        public void SetTint(Microsoft.Maui.Graphics.Color? color)
        {
            if (!hasHandler)
                return;

            if (color != null)
            {
                var src = PorterDuff.Mode.SrcIn ?? throw new InvalidOperationException("PorterDuff.Mode.SrcIn should not be null at runtime.");
                var port = new PorterDuffColorFilter(color.ToPlatform(), src);
                PlatformView?.SetColorFilter(port);
            }
            else
            {
                PlatformView?.SetColorFilter(null);
            }
        }

        protected override void ConnectHandler(ImageView platformView)
        {
            base.ConnectHandler(platformView);
            hasHandler = true;
        }

        protected override void DisconnectHandler(ImageView platformView)
        {
            hasHandler = false;
            base.DisconnectHandler(platformView);
        }
    }
}
