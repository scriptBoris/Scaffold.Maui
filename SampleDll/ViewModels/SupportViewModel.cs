using SampleDll.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleDll.ViewModels;

public class SupportViewModelKey
{
}

public class SupportViewModel : BaseViewModel<SupportViewModelKey>
{
    public ICommand CommandAlert => new Command(() =>
    {
        ShowAlert("Message", "Clicked!", "OK");
    });
}
