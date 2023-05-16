using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ScaffoldLib.Maui.Internal;

//internal partial class ImageTintHandler
//{
//    private bool hasHandler;
//    private Microsoft.UI.Xaml.Media.ImageSource? imageSource;
//    private ImageTint Proxy => (ImageTint)VirtualView;

//    public override void SetVirtualView(IView view)
//    {
//        base.SetVirtualView(view);
//        SetTint(Proxy.TintColor);
//    }

//    protected override void ConnectHandler(Microsoft.UI.Xaml.Controls.Image platformView)
//    {
//        hasHandler = true;
//        base.ConnectHandler(platformView);
//    }

//    protected override void DisconnectHandler(Microsoft.UI.Xaml.Controls.Image platformView)
//    {
//        hasHandler = false;
//        base.DisconnectHandler(platformView);
//    }

//    public async void SetTint(Microsoft.Maui.Graphics.Color? color)
//    {
//        if (!hasHandler)
//            return;

//        imageSource = PlatformView.Source;

//        if (color != null)
//        {
//            try
//            {
//                var cast = PlatformView.Source;
//                switch (cast)
//                {
//                    case BitmapImage winUI_bmp:
//                        var file = await StorageFile.GetFileFromApplicationUriAsync(winUI_bmp.UriSource);
//                        if (file != null)
//                        {
//                            var stream = await file.OpenStreamForReadAsync();
//                            var bmp = SKBitmap.Decode(stream);
//                            int w = bmp.Width;
//                            int h = bmp.Height;
//                            var canvas = new SKCanvas(bmp);
//                            var tintPaint = new SKPaint();
//                            tintPaint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Red, SKBlendMode.Dst);
//                            canvas.DrawBitmap(bmp, 0, 0, tintPaint);

//                            var bin = bmp.Bytes;
//                            //var str = new InMemoryRandomAccessStream();
//                            //await str.WriteAsync(bin.AsBuffer());

//                            var sb = SoftwareBitmap.CreateCopyFromBuffer(bin.AsBuffer(), BitmapPixelFormat.Bgra8, h, w);

//                            PlatformView.DispatcherQueue.TryEnqueue(async () =>
//                            {
//                                var ss = new SoftwareBitmapSource();
//                                await ss.SetBitmapAsync(sb);
//                                PlatformView.Source = ss;
//                            });
//                        }

//                        break;
//                    default:
//                        break;
//                }
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine(ex.ToString());
//            }
//        }
//        else
//        {
//            PlatformView.Source = imageSource;
//        }
//    }

//    private async void Bmp_ImageOpened(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
//    {
//    }
//}
