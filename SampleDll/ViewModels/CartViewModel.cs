using SampleDll.Core;
using SampleDll.Models;
using SampleDll.Services;
using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleDll.ViewModels;

public class CartViewModelKey
{
}

public class CartViewModel : BaseViewModel<CartViewModelKey>, IRemovedFromNavigation
{
    private readonly ICartService _cartService;

    public CartViewModel(ICartService cartService)
    {
        _cartService = cartService;
        Items = cartService.Items;
        TotalItems = cartService.TotalItems;
        TotalSum = cartService.TotalPrice;

        CommandItemMinus = new Command<CartItem>(ActionItemMinus);
        CommandItemPlus = new Command<CartItem>(ActionItemPlus);

        _cartService.TotalItemsChanged += CartService_TotalItemsChanged;
        _cartService.TotalPriceChanged += CartService_TotalPriceChanged;
    }

    #region props
    public int TotalItems { get; set; }
    public decimal TotalSum { get; set; }
    public IReadOnlyCollection<CartItem> Items { get; set; }
    #endregion props

    #region commands
    public ICommand CommandItemMinus { get; set; }
    private void ActionItemMinus(CartItem item)
    {
        _cartService.Decrement(item.Id, 1);
    }

    public ICommand CommandItemPlus { get; set; }
    private void ActionItemPlus(CartItem item)
    {
        _cartService.Increment(item.Id, 1);
    }
    #endregion commands

    private void CartService_TotalPriceChanged(object? sender, EventArgs e)
    {
        TotalSum = _cartService.TotalPrice;
    }

    private void CartService_TotalItemsChanged(object? sender, EventArgs e)
    {
        TotalItems = _cartService.TotalItems;
    }

    public void OnRemovedFromNavigation()
    {
        _cartService.TotalItemsChanged -= CartService_TotalItemsChanged;
        _cartService.TotalPriceChanged -= CartService_TotalPriceChanged;
    }
}
