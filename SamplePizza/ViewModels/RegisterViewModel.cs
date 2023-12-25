using SamplePizza.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ScaffoldLib.Maui;
using ScaffoldLib.Maui.Core;

namespace SamplePizza.ViewModels;

public class RegisterViewModelKey
{
}

public class RegisterViewModel : BaseViewModel<RegisterViewModelKey>, IBackButtonListener
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime? Birthday { get; set; }
    public string? SelectedLanguage { get; set; }

    public List<string> Languages { get; set; } = new()
    {
        "English",
        "Russian",
        "Urkaine",
        "Kazakhstan",
        "Uzbekistan",
        "Spanish",
        "Italian",
        "Chinese",
        "Japanese",
    };

    public ICommand CommandAccept => new Command(() =>
    {
        GoTo(new ConfirmEmailViewModelKey());
    });

    public async Task<bool> OnBackButton()
    {
        if (!HasChanges())
            return true;

        bool res = await ShowAlert("Warning", "You are sure to return previus page?", "Go back", "Cancel");
        return res;
    }

    private bool HasChanges()
    {
        if (!string.IsNullOrWhiteSpace(FirstName))
            return true;
        if (!string.IsNullOrWhiteSpace(LastName))
            return true;
        if (!string.IsNullOrWhiteSpace(Email))
            return true;
        if (!string.IsNullOrWhiteSpace(Password))
            return true;
        return false;
    }
}
