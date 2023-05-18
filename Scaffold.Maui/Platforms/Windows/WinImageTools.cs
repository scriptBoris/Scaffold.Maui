using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ScaffoldLib.Maui.Platforms.Windows;

internal static class WinImageTools
{
    public class ImgResult
    {
        public ImageSource? Source { get; set; }
        public bool IsCanceled { get; set; }

        public static ImgResult Result(ImageSource? result) => new ImgResult { Source = result };
    }

    public static async Task<ImgResult> ProcessImage(ImageSource? source, Color? tintColor, CancellationToken cancel)
    {
        if (source == null || tintColor == null)
            return ImgResult.Result(source);

        var serviceProvider = Application.Current?
            .Handler?
            .MauiContext?
            .Services
            .GetService<IImageSourceServiceProvider>()?
            .GetImageSourceService(source);

        if (serviceProvider == null)
            return ImgResult.Result(null);

        var imageSourceValue = await serviceProvider.GetImageSourceAsync(source);
        var winImageSource = imageSourceValue?.Value;

        if (winImageSource is Microsoft.UI.Xaml.Media.Imaging.BitmapImage img)
        {
            var ff = await StorageFile.GetFileFromApplicationUriAsync(img.UriSource);
            if (ff != null)
            {
                using var stream = await ff.OpenStreamForReadAsync();
                using var bmp = SKBitmap.Decode(stream);
                int w = bmp.Width;
                int h = bmp.Height;

                using var surface = SKSurface.Create(new SKImageInfo(w,h));
                using var canvas = surface.Canvas;
                using var tintPaint = new SKPaint
                {
                    ColorFilter = SKColorFilter.CreateBlendMode(tintColor.ToSkia(), SKBlendMode.SrcIn),
                };
                canvas.DrawBitmap(bmp, 0, 0, tintPaint);

                var bin = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).ToArray();

                if (cancel.IsCancellationRequested)
                    return new ImgResult { IsCanceled = true };

                return new ImgResult
                {
                    Source = ImageSource.FromStream(() => new MemoryStream(bin)),
                };
            }
        }

        if (cancel.IsCancellationRequested)
            return new ImgResult { IsCanceled = true };

        return ImgResult.Result(null);
    }

    private static SKColor ToSkia(this Color mauiColor)
    {
        byte red = (byte)(mauiColor.Red * 255f);
        byte green = (byte)(mauiColor.Green * 255f);
        byte blue = (byte)(mauiColor.Blue * 255f);
        byte alpha = (byte)(mauiColor.Alpha * 255f);

        // Создайте экземпляр SKColor из компонентов цвета
        var skiaColor = new SKColor(red, green, blue, alpha);
        return skiaColor;
    }
}
