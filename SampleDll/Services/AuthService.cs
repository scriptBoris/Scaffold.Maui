using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using SampleDll.ViewModels;
using ScaffoldLib.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDll.Services;

public interface IAuthService
{
    bool IsUserLoggin { get; }
    void SetupAppForLogin();
    void SetupAppForUnauthorized();
    void Logout();
}

public class AuthService : IAuthService
{
    private readonly IServiceProvider _serviceProvider;
    private IServiceScope? sessionScope;

    public AuthService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        string? token = Preferences.Default.Get<string?>("isUserLogin", null);
        IsUserLoggin = token != null;
    }

    public bool IsUserLoggin { get; private set; }

    public void SetupAppForLogin()
    {
        sessionScope?.Dispose();
        IsUserLoggin = true;
        Preferences.Default.Set("isUserLogin", "true");
        sessionScope = _serviceProvider.CreateScope();

        var scaffold = Scaffold.GetRootScaffold()!;
        var masterVm = sessionScope
            .ServiceProvider
            .GetRequiredService<INavigationMap>()
            .Resolve(new MasterViewModelKey());

        if (scaffold.NavigationStack.Count > 0)
            scaffold.PopToRootAndSetRootAsync(masterVm.View);
        else
            scaffold.PushAsync(masterVm.View);
    }

    public void SetupAppForUnauthorized()
    {
        sessionScope?.Dispose();
        sessionScope = _serviceProvider.CreateScope();
        var rootScaffold = Scaffold.GetRootScaffold()!;
        var loginVm = sessionScope
            .ServiceProvider
            .GetRequiredService<INavigationMap>()
            .Resolve(new LoginViewModelKey());

        if (rootScaffold.NavigationStack.Count > 0)
            rootScaffold.PopToRootAndSetRootAsync(loginVm.View);
        else
            rootScaffold.PushAsync(loginVm.View);
    }

    public void Logout()
    {
        Preferences.Default.Remove("isUserLogin");
        IsUserLoggin = false;
        SetupAppForUnauthorized();
    }
}
