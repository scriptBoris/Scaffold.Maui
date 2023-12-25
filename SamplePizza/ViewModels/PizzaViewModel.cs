using PropertyChanged;
using SamplePizza.Core;
using SamplePizza.Models;
using SamplePizza.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePizza.ViewModels;

public class PizzaViewModelKey
{
    public required PizzaItem PizzaItem { get; set; }
}

public class PizzaViewModel : BaseViewModel<PizzaViewModelKey>
{
    private readonly ICartService _cartService;

    public PizzaViewModel(ICartService cartService)
    {
        _cartService = cartService;
        PizzaItem = Args.PizzaItem;
        Count = cartService.Items.FirstOrDefault(x => x.Id == PizzaItem.Id)?.Count ?? 1;
    }

    #region props
    public PizzaItem PizzaItem { get; set; }
    public int Count { get; set; } = 1;

    public bool IsInCart => _cartService.Items.Any(x => x.Id == PizzaItem.Id);

    [DependsOn(nameof(Count))]
    public decimal Sum => Count * PizzaItem.Price;
    #endregion props

    #region commands
    public ICommand CommandLike => new Command(() =>
    {
        PizzaItem.IsLiked = !PizzaItem.IsLiked;
    });

    public ICommand CommandAddToCart => new Command(() =>
    {
        _cartService.AddToCart(PizzaItem, Count);
        OnPropertyChanged(nameof(IsInCart));
    });

    public ICommand CommandPlus => new Command(() =>
    {
        Count++;
    });

    public ICommand CommandMinus => new Command(() =>
    {
        if (Count > 1)
            Count--;
    });
    #endregion commands
}
