using SamplePizza.Core;
using SamplePizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePizza.ViewModels;

public class PizzaViewModel : BaseViewModel<Views.PizzaView>
{
    public required PizzaItem PizzaItem { get; set; }

    public ICommand CommandLike => new Command(() =>
    {
        PizzaItem.IsLiked = !PizzaItem.IsLiked;
    });
}
