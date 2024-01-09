using ScaffoldLib.Maui;
using SampleDll.Services;
using SampleDll.Controls;

namespace SampleDll;

public static class SampleDllInit
{
    public static void EntryPoint(IAuthService authService)
    {
        Application.Current!.MainPage = new MainPage();

        if (authService.IsUserLoggin)
            authService.SetupAppForLogin();
        else
            authService.SetupAppForUnauthorized();
    }

    public static MauiAppBuilder UseSampleDll(this MauiAppBuilder builder)
    {
        builder.UseScaffold();

        var services = builder.Services;

        // singletons
        services.AddSingleton<IAuthService, AuthService>();

        // scoped
        services.AddScoped<INavigationMap, NavigationMap>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IWaresService, WaresService>();

        builder.ConfigureMauiHandlers(h =>
         {
#if WINDOWS
             h.AddHandler(typeof(ElementPicker), typeof(ElementPickerHandler));
#endif
         });

        return builder;
    }
}