using SamplePizza.Core;
using ScaffoldLib.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePizza.ViewModels;

public class AccountViewModel : BaseViewModel<Views.AccountView>
{
    public ICommand CommandLogout => new Command(async () =>
    {
        bool exit = await ShowAlert("Logout", "Are you sure you want to exit from current user profile?", "Exit", "Cancel");
        if (exit)
            Scaffold.GetRootScaffold()?.PopToRootAndSetRootAsync(new LoginViewModel().View);
    });

    public ICommand CommandDeleteAccount => new Command(async () =>
    {
        bool exit = await ShowAlert("Warning", "Are you sure you want to delete your profile? This action is irreversible", "Delete", "Cancel");
        if (exit) 
            Scaffold.GetRootScaffold()?.PopToRootAndSetRootAsync(new LoginViewModel().View);
    });
}
