using SamplePizza.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SamplePizza.Models;
using SamplePizza.Services;

namespace SamplePizza.ViewModels;

public class HomeViewModelKey
{
}

public class HomeViewModel : BaseViewModel<HomeViewModelKey>
{
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
    private void ActionSelectPizza(PizzaItem item)
    {
        GoTo(new PizzaViewModelKey()
        {
            PizzaItem = item
        });
    }
}