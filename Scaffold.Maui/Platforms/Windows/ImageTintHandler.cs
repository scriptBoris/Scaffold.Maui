using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScaffoldLib.Maui.Toolkit;

namespace ScaffoldLib.Maui.Platforms.Windows;

#if NET7_0
public class ImageTintHandler : ImageHandler
{
    private ImageSourcePartLoader? _imageSourcePartLoader;

    public ImageTintHandler() : base(ImageTintHandlerMapper)
    {
    }

    public static PropertyMapper<ImageTint, ImageTintHandler> ImageTintHandlerMapper = new(Mapper)
    {
        [nameof(ImageTint.Source)] = MapSource,
        [nameof(ImageTint.TintColor)] = MapTintColor,
    };

    public ImageSourcePartLoader Loader
    {
        get
        {
            _imageSourcePartLoader ??= new ImageSourcePartLoader(this, GetImagePart, SetImage);
            return _imageSourcePartLoader;
        }
    }

    protected virtual IImageSourcePart? GetImagePart()
    {
        return VirtualView;
    }

    private CancellationTokenSource? cancelSource;
    private Microsoft.UI.Xaml.Media.ImageSource? originImageSource;

    protected virtual void SetImage(Microsoft.UI.Xaml.Media.ImageSource? imageSource)
    {
        originImageSource = imageSource;
        ProcessTintAndSetup(imageSource);
    }

    protected virtual async void ProcessTintAndSetup(Microsoft.UI.Xaml.Media.ImageSource? imageSource)
    {
        cancelSource?.Cancel();
        cancelSource = new();

        if (VirtualView is not ImageTint proxy)
            return;

        var res = await Platforms.Windows.WinImageTools.ProcessImage(imageSource, proxy.TintColor, cancelSource.Token);
        if (res.IsCanceled)
            return;

        PlatformView.Source = res.Source;
    }

    public static void MapSource(ImageTintHandler h, ImageTint v)
    {
        h.Loader.UpdateImageSourceAsync();
    }

    public static void MapTintColor(ImageTintHandler h, ImageTint v)
    {
        h.ProcessTintAndSetup(h.originImageSource);
    }
}

#else
public class ImageTintHandler : ImageHandler
{
    private ImageSourcePartLoader? _imageSourcePartLoader;

    public ImageTintHandler() : base(ImageTintHandlerMapper)
    {
    }

    public static PropertyMapper<ImageTint, ImageTintHandler> ImageTintHandlerMapper = new(Mapper)
    {
        [nameof(ImageTint.Source)] = MapSource,
        [nameof(ImageTint.TintColor)] = MapTintColor,
    };

    public ImageSourcePartLoader Loader
    {
        get
        {
            //_imageSourcePartLoader ??= new ImageSourcePartLoader(this, GetImagePart, SetImage);
            if (_imageSourcePartLoader == null)
            {
                var str = new SetterImg
                {
                    Parent = this,
                    Handler = this,
                    ImageSourcePart = VirtualView,
                };
                _imageSourcePartLoader = new ImageSourcePartLoader(str);
            }
            return _imageSourcePartLoader;
        }
    }

    private CancellationTokenSource? cancelSource;
    private Microsoft.UI.Xaml.Media.ImageSource? originImageSource;

    protected virtual async void ProcessTintAndSetup(Microsoft.UI.Xaml.Media.ImageSource? imageSource)
    {
        cancelSource?.Cancel();
        cancelSource = new();

        if (VirtualView is not ImageTint proxy)
            return;

        var res = await Platforms.Windows.WinImageTools.ProcessImage(imageSource, proxy.TintColor, cancelSource.Token);
        if (res.IsCanceled)
            return;

        PlatformView.Source = res.Source;
    }

    protected virtual async void ProcessTintAndSetup(StreamImageSource imageSource)
    {
        cancelSource?.Cancel();
        cancelSource = new();

        if (VirtualView is not ImageTint proxy)
            return;

        Stream? str = null;
        try
        {
            str = await imageSource.Stream(cancelSource.Token);
        }
        catch (Exception)
        {
        }
        if (str == null)
            return;

        var res = Platforms.Windows.WinImageTools.ProcessImage(str, proxy.TintColor, cancelSource.Token);
        if (res.IsCanceled)
            return;

        PlatformView.Source = res.Source;
    }

    public static void MapSource(ImageTintHandler h, ImageTint v)
    {
        if (v.Source is StreamImageSource streamImageSource)
        {
            h.ProcessTintAndSetup(streamImageSource);
        }
        else
        {
            h.Loader.UpdateImageSourceAsync();
        }
    }

    public static void MapTintColor(ImageTintHandler h, ImageTint v)
    {
        if (v.Source is StreamImageSource streamImageSource)
        {
            h.ProcessTintAndSetup(streamImageSource);
        }
        else
        {
            h.ProcessTintAndSetup(h.originImageSource);
        }
    }

    private class SetterImg : IImageSourcePartSetter
    {
        public required ImageTintHandler Parent { get; set; }
        public IElementHandler? Handler { get; set; }
        public IImageSourcePart? ImageSourcePart { get; set; }

        public void SetImageSource(Microsoft.UI.Xaml.Media.ImageSource? platformImage)
        {
            Parent.originImageSource = platformImage;
            Parent.ProcessTintAndSetup(platformImage);
        }
    }
}
#endif