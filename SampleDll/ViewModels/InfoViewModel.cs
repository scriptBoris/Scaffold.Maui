using SampleDll.Core;
using ScaffoldLib.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleDll.ViewModels;

public class InfoViewModelKey
{
}

public class InfoViewModel : BaseViewModel<InfoViewModelKey>
{
    public ICommand CommandGoRepo => new Command(() =>
    {
        Browser.OpenAsync("https://github.com/scriptboris/scaffold.maui");
    });

    public ICommand CommandFeedback => new Command(() =>
    {
        View.GetContext()?.DisplayActionSheet(new ScaffoldLib.Maui.Args.DisplayActionSheet
        {
            Title = "Feedback",
            Cancel = "Cancel",
            Items = 
            [
                "Fine",
                "Orange emotions",
                "Hello world!",
            ],
            SelectedItemId = 0,
            UseShowAnimation = true,
        });
    });
}
