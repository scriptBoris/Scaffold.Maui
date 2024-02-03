using CoreGraphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using ScaffoldLib.Maui.Toolkit;
using Microsoft.Maui.Graphics.Platform;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui.Platforms.iOS;

public class ImageTintHandler : ImageHandler
{
    private ImageSourcePartLoader? _imageSourcePartLoader;

    public ImageTintHandler() : base(ImageTintHandlerMapper)
    {
    }

    public static PropertyMapper<ImageTint, ImageTintHandler> ImageTintHandlerMapper = new(Mapper)
    {
        [nameof(ImageTint.TintColor)] = MapTintColor,
    };

    public static void MapTintColor(ImageTintHandler h, ImageTint v)
    {
        var native = h.PlatformView as UIImageTint;
        var proxy = h.VirtualView as ImageTint;

        if (native != null && proxy != null)
        {
            native.ColorFilter = proxy.TintColor?.ToPlatform();
        }
    }

    //public override ImageSourcePartLoader SourceLoader =>
    //    _imageSourcePartLoader ??= new ImageSourcePartLoaderExt(new Loader(this));

    protected override UIImageView CreatePlatformView()
    {
        return new UIImageTint();
    }

    public class UIImageTint : MauiImageView
    {
        private UIColor? _colorFilter;
        private UIImage? originalImage;
        private UIImage? filteredImage;

        public UIImageTint() : base()
        {
        }

        public UIColor? ColorFilter
        {
            get => _colorFilter;
            set
            {
                var old = _colorFilter;
                _colorFilter = value;

                if (old != value)
                    UpdateColorFilter();
            }
        }

        public override UIImage? Image
        {
            get => base.Image;
            set
            {
                base.Image = value;
                originalImage = value;
                UpdateColorFilter();
            }
        }

        private void UpdateColorFilter()
        {
            if (originalImage == null)
                return;

            if (ColorFilter != null)
            {
                filteredImage = ApplyTintToImage(originalImage, ColorFilter);
                base.Image = filteredImage;
            }
            else
            {
                base.Image = originalImage;
            }
        }

        public static UIImage ApplyTintToImage(UIImage image, UIColor tintColor)
        {
            UIGraphics.BeginImageContextWithOptions(image.Size, false, image.CurrentScale);

            using CGContext context = UIGraphics.GetCurrentContext();
            var rect = new CGRect(0, 0, image.Size.Width, image.Size.Height);

            // Оригинальное изображение
            image.Draw(rect, CGBlendMode.Normal, 1.0f);

            // Применяем цветной слой с учетом альфа-канала
            context.SetBlendMode(CGBlendMode.SourceIn);
            context.SetFillColor(tintColor.CGColor);
            context.FillRect(rect);

            // Получаем изображение с двумя слоями
            UIImage tintedImage = UIGraphics.GetImageFromCurrentImageContext();

            // Завершаем контекст
            UIGraphics.EndImageContext();

            var res = CombineImages(image, tintedImage);
            return res;
        }

        public static UIImage CombineImages(UIImage image1, UIImage image2)
        {
            var size = new CGSize(Math.Max(image1.Size.Width, image2.Size.Width), Math.Max(image1.Size.Height, image2.Size.Height));

            UIGraphics.BeginImageContextWithOptions(size, false, UIScreen.MainScreen.Scale);

            var imageRect1 = new CGRect(0, 0, image1.Size.Width, image1.Size.Height);
            var imageRect2 = new CGRect(0, 0, image2.Size.Width, image2.Size.Height);

            image1.Draw(imageRect1);
            image2.Draw(imageRect2);

            var combinedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return combinedImage;
        }
    }

    //public class ImageSourcePartLoaderExt : ImageSourcePartLoader
    //{
    //    public ImageSourcePartLoaderExt(IImageSourcePartSetter str) : base(str)
    //    {
    //    }
    //}

    //public class Loader : IImageSourcePartSetter
    //{
    //    private readonly WeakReference<ImageTintHandler> _handler;

    //    public Loader(ImageTintHandler handler)
    //    {
    //        _handler = new(handler);
    //    }

    //    public IImageSourcePart? ImageSourcePart =>
    //        Handler?.VirtualView as IImageSourcePart ?? Handler?.VirtualView as Microsoft.Maui.IImage;

    //    public IElementHandler? Handler => _handler.GetTargetOrDefault();

    //    public void SetImageSource(UIImage? platformImage)
    //    {
    //        if (Handler?.PlatformView is not UIImageView imageView)
    //            return;

    //        imageView.Image = platformImage;
    //        if (Handler?.VirtualView is Microsoft.Maui.IImage image && image.Source is IStreamImageSource)
    //            imageView.InvalidateMeasure(image);
    //    }
    //}
}