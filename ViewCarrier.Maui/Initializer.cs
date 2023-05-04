using ButtonSam.Maui;
using Scaffold.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui
{
    public static class Initializer
    {
        public static MauiAppBuilder UseViewCarrier(this MauiAppBuilder builder)
        {
#if ANDROID
            Platforms.Android.Initializer.Init(builder);
#endif

            builder.ConfigureMauiHandlers(h =>
            {
                h.AddHandler(typeof(ImageTint), typeof(ImageTintHandler));
            });

            builder.UseButtonSam();
            return builder;
        }
    }
}
