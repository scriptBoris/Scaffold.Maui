using SamplePizza.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePizza.ViewModels;

public class InfoViewModelKey
{
}

public class InfoViewModel : BaseViewModel<InfoViewModelKey>
{
    public ICommand CommandGoRepo => new Command(() =>
    {
        Browser.OpenAsync("https://github.com/scriptboris/scaffold.maui");
    });
}
