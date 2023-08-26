using Sample.Models;
using ScaffoldLib.Maui;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Sample.Views.Pizza;

public partial class HomeView
{
    public HomeView()
    {
        InitializeComponent();
        BindingContext = this;
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
                Name = "Ñheese",
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
        this.GetContext()?.PushAsync(new PizzaView(item));
    }
}