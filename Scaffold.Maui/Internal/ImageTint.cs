using Microsoft.Maui.Controls;
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
#if ANDROID
                if (b is ImageTint self && self.Handler is ImageTintHandler h) h.SetTint(n as Color);
#elif WINDOWS
                if (b is ImageTint self) self.UpdateSource();
#endif
            }
        );
        public Color? TintColor
        {
            get => GetValue(TintColorProperty) as Color;
            set => SetValue(TintColorProperty, value);
        }

#if WINDOWS
        public static new readonly BindableProperty SourceProperty = BindableProperty.Create(
            nameof(Source),
            typeof(ImageSource),
            typeof(ImageTint),
            null,
            propertyChanged:(b,o,n) =>
            {
                if (b is ImageTint self) self.UpdateSource();
            }
        );
        public new ImageSource? Source
        {
            get => GetValue(SourceProperty) as ImageSource;
            set => SetValue(SourceProperty, value);
        }

        private CancellationToken cancel;
        private async void UpdateSource()
        {
            cancel.ThrowIfCancellationRequested();
            cancel = new CancellationToken();
            var res = await Platforms.Windows.WinImageTools.Handle(Source, TintColor, cancel);
            if (res.IsCanceled)
                return;

            base.Source = res.Source;
        }
#endif
    }
}
