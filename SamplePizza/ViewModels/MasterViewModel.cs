using SamplePizza.Core;
using SamplePizza.Models;
using SamplePizza.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePizza.ViewModels;

public class MasterViewModelKey
{
}

public class MasterViewModel : BaseViewModel<MasterViewModelKey>
{
    private readonly INavigationMap _navigationMap;

    public MasterViewModel(INavigationMap navigationMap)
    {
        _navigationMap = navigationMap;

        Menus = new()
        {
            new MasterMenuItem
            {
                Title = "Home",
                ImageSource = "home.png",
                ViewModelKey = new HomeViewModelKey(),
            },
            new MasterMenuItem
            {
                Title = "Account",
                ImageSource = "account.png",
                ViewModelKey = new AccountViewModelKey(),
            },
            new MasterMenuItem
            {
                Title = "About app",
                ImageSource = "information_slab_circle.png",
                ViewModelKey = new InfoViewModelKey(),
            },
            new MasterMenuItem
            {
                Title = "Technical support",
                ImageSource = "headset.png",
                ViewModelKey = new SupportViewModelKey(),
            },
            new MasterMenuItem
            {
                Title = "Settings",
                ImageSource = "cog.png",
                ViewModelKey = new SettingsViewModelKey(),
                LayoutOptions = LayoutOptions.EndAndExpand,
            },
        };

        SelectedMenuButton = SelectMenu(Menus.First());
    }

    public ObservableCollection<MasterMenuItem> Menus { get; set; }
    public MasterMenuItem SelectedMenuButton { get; set; }
    public bool IsPresented { get; set; }

    public ICommand CommandSelectMenu => new Command<MasterMenuItem>((param) =>
    {
        SelectedMenuButton = SelectMenu(param);
    });

    private MasterMenuItem SelectMenu(MasterMenuItem item)
    {
        item.ViewModel ??= _navigationMap.Resolve(item.ViewModelKey);

        if (SelectedMenuButton != null)
            SelectedMenuButton.IsSelected = false;
        
        item.IsSelected = true;

#if !WINDOWS
        IsPresented = false;
#endif

        return item;
    }
}