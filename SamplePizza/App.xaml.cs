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
}