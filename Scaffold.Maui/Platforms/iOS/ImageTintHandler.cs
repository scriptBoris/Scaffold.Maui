using CoreGraphics;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ScaffoldLib.Maui.Internal;

public partial class ImageTintHandler
{
    private bool hasHandler;
    private ImageTint Proxy => (ImageTint)VirtualView;
    private UIImageTint? Native => PlatformView as UIImageTint;

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        SetTint(Proxy.TintColor);
    }

    public void SetTint(Microsoft.Maui.Graphics.Color? color)
    {
        if (!hasHandler)
            return;

        if (Native == null)
            return;

        Native.ColorFilter = color?.ToPlatform();

        //if (color != null)
        //{
        //    PlatformView.Image = PlatformView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        //    PlatformView.TintColor = color.ToPlatform();
        //}
        //else
        //{
        //    PlatformView.Image = PlatformView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
        //}
    }

    protected override UIImageView CreatePlatformView()
    {
        return new UIImageTint();
    }

    protected override void ConnectHandler(UIImageView platformView)
    {
        base.ConnectHandler(platformView);
        hasHandler = true;
    }

    protected override void DisconnectHandler(UIImageView platformView)
    {
        hasHandler = false;
        base.DisconnectHandler(platformView);
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
}
