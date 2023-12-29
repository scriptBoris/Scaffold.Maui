using SamplePizza.Services;
using SamplePizza.ViewModels;

namespace SamplePizza;

public partial class App : Application
{
    public App(IAuthService authService)
    {
        InitializeComponent();
        MainPage = new MainPage();

        if (authService.IsUserLoggin)
            authService.SetupAppForLogin();
        else
            authService.SetupAppForUnauthorized();
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var w = base.CreateWindow(activationState);
        w.Width = 500;
        w.Height = 700;
        w.X = 700;
        return w;
    }
#endif
}