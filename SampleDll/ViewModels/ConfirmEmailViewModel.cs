using SampleDll.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ScaffoldLib.Maui;
using ScaffoldLib.Maui.Core;

namespace SampleDll.ViewModels;

public class ConfirmEmailViewModelKey
{
}

public class ConfirmEmailViewModel : BaseViewModel<ConfirmEmailViewModelKey>, IAppear
{
    private const string code = "618952";
    public string? ConfirmCode { get; set; }

    public ICommand CommandRetry => new Command(() =>
    {
        ShowToastCode();
    });

    public ICommand CommandConfirm => new Command(() =>
    {
        if (ConfirmCode == code)
        {
            GoBackToRoot();
        }
        else
        {
            ShowError("Not valid confirm code");
        }
    });

    private void ShowToastCode()
    {
        ShowToast("Confirm code", $"{code}");
    }

    public void OnAppear(bool isComplete)
    {
        if (isComplete)
        {
            ShowToastCode();
        }
    }
}
