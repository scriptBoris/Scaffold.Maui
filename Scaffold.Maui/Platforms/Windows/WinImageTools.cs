using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using WinImageSource = Microsoft.UI.Xaml.Media.ImageSource;

namespace ScaffoldLib.Maui.Platforms.Windows;

internal static class WinImageTools
{
    public class ImgResult
    {
        public WinImageSource? Source { get; set; }
        public bool IsCanceled { get; set; }

        public static ImgResult Result(WinImageSource? result) => new ImgResult { Source = result };
        public static ImgResult Cancel() => new ImgResult { IsCanceled = true };
    }

    public static async Task<ImgResult> ProcessImage(WinImageSource? source, Color? tintColor, CancellationToken cancel)
    {
        if (source == null)
            return ImgResult.Result(source);

        if (source is Microsoft.UI.Xaml.Media.Imaging.BitmapImage img)
        {
            var ff = await StorageFile.GetFileFromApplicationUriAsync(img.UriSource);
            if (cancel.IsCancellationRequested)
                return ImgResult.Cancel();

            if (ff != null)
            {
                using var stream = await ff.OpenStreamForReadAsync();
                using var bmp = SKBitmap.Decode(stream);
                int w = bmp.Width;
                int h = bmp.Height;

                using var surface = SKSurface.Create(new SKImageInfo(w,h));
                using var canvas = surface.Canvas;

                if (tintColor != null)
                {
                    using var tintPaint = new SKPaint
                    {
                        ColorFilter = SKColorFilter.CreateBlendMode(tintColor.ToSkia(), SKBlendMode.SrcIn),
                    };
                    canvas.DrawBitmap(bmp, 0, 0, tintPaint);
                }
                else
                {
                    canvas.DrawBitmap(bmp, 0, 0);
                }

                var bin = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).ToArray();
                if (cancel.IsCancellationRequested)
                    return ImgResult.Cancel();

                IRandomAccessStream streamResult = new MemoryStream(bin).AsRandomAccessStream();
                var sourceResult = new WriteableBitmap(w, h);
                sourceResult.SetSource(streamResult);

                return ImgResult.Result(sourceResult);
            }
        }

        if (cancel.IsCancellationRequested)
            return ImgResult.Cancel();

        return ImgResult.Result(null);
    }

    private static SKColor ToSkia(this Color mauiColor)
    {
        byte red = (byte)(mauiColor.Red * 255f);
        byte green = (byte)(mauiColor.Green * 255f);
        byte blue = (byte)(mauiColor.Blue * 255f);
        byte alpha = (byte)(mauiColor.Alpha * 255f);

        var skiaColor = new SKColor(red, green, blue, alpha);
        return skiaColor;
    }
}
