using SamplePizza.Core;
using SamplePizza.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePizza.ViewModels;

public class LoginViewModelKey
{
}

public class LoginViewModel : BaseViewModel<LoginViewModelKey>
{
    private readonly IAuthService _authService;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    public string? Login { get; set; }
    public string? Password { get; set; }

    public ICommand CommandLogin => new Command(() =>
    {
        _authService.SetupAppForLogin();
    });

    public ICommand CommandRegister => new Command(() =>
    {
        GoTo(new RegisterViewModelKey());
    });
}
