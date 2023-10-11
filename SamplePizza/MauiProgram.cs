using Microsoft.Extensions.Logging;
using SamplePizza.Controls;
using ScaffoldLib.Maui;

namespace SamplePizza
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseScaffold()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(h =>
                {
#if WINDOWS
                    h.AddHandler(typeof(ElementPicker), typeof(ElementPickerHandler));
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}