﻿using ScaffoldLib.Maui;
using SampleDll.Services;
using SampleDll.Controls;
using ScaffoldLib.Maui.StaticLibs.ButtonSam;
using ButtonSam.Maui;

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
        builder.UseScaffold(new ScaffoldLib.Maui.Args.UseScaffoldArgs
        {
            UseDebugInfo = true,
        });

        builder.UseButtonSam();

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