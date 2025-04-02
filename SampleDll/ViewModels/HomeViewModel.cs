using SampleDll.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SampleDll.Models;
using SampleDll.Services;

namespace SampleDll.ViewModels;

public class HomeViewModelKey
{
}

public class HomeViewModel : BaseViewModel<HomeViewModelKey>
{
    private bool isBusy;

    public HomeViewModel(IWaresService waresService)
    {
        CommandSelectPizza = new Command<PizzaItem>(ActionSelectPizza);

        PizzaItems = new ObservableCollection<PizzaItem>(waresService.GetAllWares());

        Filters = new()
        {
            new HomeFilterItem
            {
                Name = "All",
            },
            new HomeFilterItem
            {
                Name = "Veg",
            },
            new HomeFilterItem
            {
                Name = "Spicy",
            },
            new HomeFilterItem
            {
                Name = "Combo",
            },
        };
    }

    public ObservableCollection<PizzaItem> PizzaItems { get; set; }
    public ObservableCollection<HomeFilterItem> Filters { get; set; }

    public ICommand CommandCart => new Command(() =>
    {
        GoTo(new CartViewModelKey());
    });

    public ICommand CommandSelectPizza { get; set; }
    private async void ActionSelectPizza(PizzaItem item)
    {
        if (isBusy)
            return;

        isBusy = true;

        if (_instances.TryGetValue(item, out var instance))
        {
            await GoTo(instance);
        }
        else
        {
            var vm = await GoTo(new PizzaViewModelKey()
            {
                PizzaItem = item
            });
            _instances.Add(item, vm);
        }
        isBusy = false;
    }

    private readonly Dictionary<PizzaItem, BaseViewModel> _instances = [];
}