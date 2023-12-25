using Microsoft.Extensions.Logging;
using SamplePizza.Controls;
using SamplePizza.Services;
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
                .ConfigureServices()
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

        private static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
        {
            var services = builder.Services;

            // singletons
            services.AddSingleton<IAuthService, AuthService>();

            // scoped
            services.AddScoped<INavigationMap, NavigationMap>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IWaresService, WaresService>();

            return builder;
        }
    }
}