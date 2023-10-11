using SamplePizza.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePizza.ViewModels;

public class LoginViewModel : BaseViewModel<Views.LoginView>
{
    public string? Login { get; set; }
    public string? Password { get; set; }

    public ICommand CommandLogin => new Command(() =>
    {
        ReplaceTo(new MasterViewModel());
    });

    public ICommand CommandRegister => new Command(() =>
    {
        GoTo(new RegisterViewModel());
    });
}
