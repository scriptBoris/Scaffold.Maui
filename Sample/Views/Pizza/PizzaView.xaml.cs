using Sample.Models;
using System.Windows.Input;

namespace Sample.Views.Pizza;

public partial class PizzaView
{
	public PizzaView(PizzaItem pizzaItem)
	{
		InitializeComponent();
        BindingContext = this;

        PizzaItem = pizzaItem;
    }

    public PizzaItem PizzaItem { get; private set; }

    public ICommand CommandLike => new Command(() =>
    {
        PizzaItem.IsLiked = !PizzaItem.IsLiked;
    });
}