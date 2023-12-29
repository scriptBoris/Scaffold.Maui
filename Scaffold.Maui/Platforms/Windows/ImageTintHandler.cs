using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit;

public partial class ImageTintHandler
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

    public static void MapSource(ImageTintHandler h, ImageTint v)
    {
        h.Loader.UpdateImageSourceAsync();
    }

    public static void MapTintColor(ImageTintHandler h, ImageTint v)
    {
        h.ProcessTintAndSetup(h.originImageSource);
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
