using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScaffoldLib.Maui.Toolkit;
using SkiaSharp;

namespace ScaffoldLib.Maui.Internal;

public class SvgImageSource : StreamImageSource
{
    public SvgImageSource()
    {
    }

    public SvgImageSource(string file)
    {
        Data = new SvgData
        {
            File = file,
            Height = 32,
            Width = 32,
        };
    }

    public SvgImageSource(string file, int width, int height)
    {
        Data = new SvgData
        {
            File = file,
            Height = width,
            Width = height,
        };
    }

    #region bindable props
    public static readonly BindableProperty DataProperty = BindableProperty.Create(
        nameof(Data),
        typeof(SvgData),
        typeof(SvgImageSource),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is SvgImageSource self)
                self.OnUpdateData(n as SvgData?);
        });
    [TypeConverter(typeof(SvgDataTypeConverter))]
    public SvgData? Data
    {
        get => GetValue(DataProperty) as SvgData?;
        set => SetValue(DataProperty, value);
    }
    #endregion bindable props

    public override Func<CancellationToken, Task<Stream>>? Stream { get; set; }

    private string ResolvePath(string path)
    {
#if IOS
        return $"ScaffoldLib.Maui.Resources.Images.{path}";
#else
        return path;
#endif
    }

    private async Task<Stream?> ResolveResource(string path)
    {
#if IOS
        var asm = Assembly.Load("Scaffold.Maui");
        var names = asm.GetManifestResourceNames();
        var stream = asm.GetManifestResourceStream(ResolvePath(path));
        return stream;
#else
        var res = await FileSystem.Current.OpenAppPackageFileAsync(path);
        return res;
#endif
    }

    private async void OnUpdateData(SvgData? data)
    {
        if (data == null)
        {
            Stream = null;
            OnSourceChanged();
            return;
        }

        try
        {
            var dat = data.Value;
            var res = await ResolveResource(dat.File);
            if (res != null)
            {
                float scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;
                //float scale = 1;
                byte[]? svg = SvgDrawer.Draw(res, scale, dat);
                if (svg != null)
                {
                    Stream = (c) =>
                    {
                        return Task.FromResult(new MemoryStream(svg) as Stream);
                    };
                }
                else
                {
                    Stream = null;
                }
            }

            OnSourceChanged();
        }
        catch (Exception ex)
        {
            string txt = $"{nameof(SvgImageSource)}:\n" + ex.ToString();
            Console.WriteLine(txt);
            System.Diagnostics.Debug.WriteLine(txt);
        }
    }

}

[TypeConverter(typeof(SvgDataTypeConverter))]
public readonly struct SvgData
{
    public required string File { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public Color? Color { get; init; }
}

public class SvgDataTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(string);

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
        var strValue = value?.ToString();

        if (strValue != null)
        {
            strValue = strValue.Trim();

            string[] split = strValue.Split(';');
            string? path = null;
            int w = 32;
            int h = 32;
            Color? color = null;

            switch (split.Length)
            {
                case 0:
                case 1:
                    path = strValue;
                    break;
                case 2:
                    path = split[0];
                    w = int.Parse(split[1]);
                    break;
                case 3:
                    path = split[0];
                    w = int.Parse(split[1]);
                    h = int.Parse(split[2]);
                    break;
                case 4:
                    path = split[0];
                    w = int.Parse(split[1]);
                    h = int.Parse(split[2]);
                    color = Color.FromRgba(split[3]);
                    break;

                default:
                    break;
            }

            return new SvgData
            {
                File = path ?? "unresolved",
                Height = h,
                Width = w,
                Color = color,
            };
        }

        throw new InvalidOperationException($"Cannot convert \"{strValue}\" into {typeof(Thickness)}");
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is not SvgData s)
            throw new NotSupportedException();

        return $"{s.File};{s.Width};{s.Height};{s.Color}";
    }
}

public static class SvgDrawer
{
    public static byte[]? Draw(Stream stream, float screenScale, SvgData svgData)
    {
        try
        {
            double width = svgData.Width;
            double height = svgData.Height;
            var color = svgData.Color ?? Colors.Red;

            var svg = new SkiaSharp.Extended.Svg.SKSvg();
            svg.Load(stream);

            var size = CalcSize(svg.Picture.CullRect.Size, width, height);
            var scale = CalcScale(svg.Picture.CullRect.Size, size, screenScale);
            var matrix = SKMatrix.CreateScale(scale.Item1, scale.Item2);

            using var bitmap = new SKBitmap((int)(size.Width * screenScale), (int)(size.Height * screenScale));
            using var canvas = new SKCanvas(bitmap);
            using var paint = new SKPaint();

            if (color != null)
                paint.ColorFilter = SKColorFilter.CreateBlendMode(ToSKColor(color), SKBlendMode.SrcIn);

            canvas.Clear(SKColors.Transparent); // very very important!
            canvas.DrawPicture(svg.Picture, ref matrix, paint);

            using var image = SKImage.FromBitmap(bitmap);
            if (image == null)
                return null;

            using var encoded = image.Encode(SKEncodedImageFormat.Png, 90);
            return encoded.ToArray();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FAIL DRAW .SVG \"{svgData.File}\"\n{ex}");
            return null;
        }
    }

    /// <summary>
    /// Tos the SKC olor.
    /// </summary>
    /// <returns>The SKC olor.</returns>
    /// <param name="color">Color.</param>
    public static SKColor ToSKColor(Color color)
    {
        return new SKColor(
            (byte)(color.Red * 255),
            (byte)(color.Green * 255),
            (byte)(color.Blue * 255),
            (byte)(color.Alpha * 255)
        );
    }

    /// <summary>
    /// Calculates the size.
    /// </summary>
    /// <returns>The size.</returns>
    /// <param name="size">Size.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    public static SKSize CalcSize(SkiaSharp.SKSize size, double width, double height)
    {
        double w;
        double h;
        if (width <= 0 && height <= 0)
        {
            return size;
        }
        else if (width <= 0)
        {
            h = height;
            w = height * (size.Width / size.Height);
        }
        else if (height <= 0)
        {
            w = width;
            h = width * (size.Height / size.Width);
        }
        else
        {
            w = width;
            h = height;
        }

        return new SkiaSharp.SKSize((float)w, (float)h);
    }

    /// <summary>
    /// Calculates the scale.
    /// </summary>
    /// <returns>The scale.</returns>
    /// <param name="originalSize">Original size.</param>
    /// <param name="scaledSize">Scaled size.</param>
    /// <param name="screenScale">Screen scale.</param>
    public static Tuple<float, float> CalcScale(SkiaSharp.SKSize originalSize, SkiaSharp.SKSize scaledSize, float screenScale)
    {
        var sx = scaledSize.Width * screenScale / originalSize.Width;
        var sy = scaledSize.Height * screenScale / originalSize.Height;

        return new Tuple<float, float>((float)sx, (float)sy);
    }
}
