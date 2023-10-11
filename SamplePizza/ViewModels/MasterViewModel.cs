using SamplePizza.Core;
using SamplePizza.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePizza.ViewModels;

public class MasterViewModel : BaseViewModel<Views.MasterView>
{
    public MasterViewModel()
    {
        Menus = new()
        {
            new MasterMenuItem
            {
                Title = "Home",
                ImageSource = "home.png",
                ViewModel = new HomeViewModel(),
            },
            new MasterMenuItem
            {
                Title = "Account",
                ImageSource = "account.png",
                ViewModel = new AccountViewModel(),
            },
            new MasterMenuItem
            {
                Title = "About app",
                ImageSource = "information_slab_circle.png",
                ViewModel = new InfoViewModel(),
            },
            new MasterMenuItem
            {
                Title = "Technical support",
                ImageSource = "headset.png",
                ViewModel = new SupportViewModel(),
            },
            new MasterMenuItem
            {
                Title = "Settings",
                ImageSource = "cog.png",
                ViewModel = new SettingsViewModel(),
                LayoutOptions = LayoutOptions.EndAndExpand,
            },
        };

        SelectedMenuButton = Menus.First();
        SelectMenu(SelectedMenuButton);
    }

    public ObservableCollection<MasterMenuItem> Menus { get; set; }
    public MasterMenuItem SelectedMenuButton { get; set; }
    public bool IsPresented { get; set; }

    public ICommand CommandSelectMenu => new Command<MasterMenuItem>((param) =>
    {
        SelectMenu(param);
    });

    private void SelectMenu(MasterMenuItem item)
    {
        SelectedMenuButton.IsSelected = false;
        item.IsSelected = true;

#if !WINDOWS
        IsPresented = false;
#endif

        SelectedMenuButton = item;
    }
}