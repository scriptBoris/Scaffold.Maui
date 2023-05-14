using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal
{
    internal partial class ImageTintHandler : ImageHandler
    {
    }

    internal class ImageTint : Image
    {
        // tint color
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
            nameof(TintColor),
            typeof(Color), 
            typeof(ImageTint),
            null,
            propertyChanged: (b,o,n) =>
            {
                if (b is ImageTint self && self.Handler is ImageTintHandler h)
                {
#if ANDROID
                    h.SetTint(n as Color);
#endif
                }
            }
        );
        public Color? TintColor
        {
            get => GetValue(TintColorProperty) as Color;
            set => SetValue(TintColorProperty, value);
        }
    }
}
