using SamplePizza.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SamplePizza.Models;

namespace SamplePizza.ViewModels;

public class HomeViewModel : BaseViewModel<Views.HomeView>
{
    public HomeViewModel()
    {
        CommandSelectPizza = new Command<PizzaItem>(ActionSelectPizza);
        
        PizzaItems = new()
        {
            new PizzaItem
            {
                Image = "pizza_1.png",
                Name = "Mixsik",
                Price = 10,
            },
            new PizzaItem
            {
                Image = "pizza_2.png",
                Name = "Chirozo fresh",
                Price = 12,
            },
            new PizzaItem
            {
                Image = "pizza_3.png",
                Name = "Burger pizza",
                Price = 13,
            },
            new PizzaItem
            {
                Image = "pizza_4.png",
                Name = "Pepperoni fresh",
                Price = 10,
            },
            new PizzaItem
            {
                Image = "pizza_5.png",
                Name = "Cheese chicken",
                Price = 14,
            },
            new PizzaItem
            {
                Image = "pizza_6.png",
                Name = "Ham and cheese",
                Price = 9,
            },
            new PizzaItem
            {
                Image = "pizza_7.png",
                Name = "Сheese",
                Price = 8,
            },
            new PizzaItem
            {
                Image = "pizza_8.png",
                Name = "Double chicken",
                Price = 16.5,
            },
        };

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

    public ICommand CommandSelectPizza { get; set; }
    private void ActionSelectPizza(PizzaItem item)
    {
        GoTo(new PizzaViewModel()
        {
            PizzaItem = item
        });
    }
}