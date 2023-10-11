using SamplePizza.Core;
using ScaffoldLib.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePizza.Models;

public class MasterMenuItem : BaseNotify
{
    private IScaffold? scaffold;

    public required BaseViewModel ViewModel { get; set; }
    public required string Title { get; set; }
    public required ImageSource ImageSource { get; set; }
    public LayoutOptions LayoutOptions { get; set; } = LayoutOptions.Start;
    public bool IsSelected { get; set; }

    public View View
    {
        get
        {
            if (scaffold == null)
            {
                scaffold = new Scaffold();
                scaffold.PushAsync(ViewModel.View);
            }
            return (View)scaffold;
        }
    }
}
