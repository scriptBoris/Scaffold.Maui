using SamplePizza.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePizza.Services;

public interface ICartService
{
    event EventHandler<EventArgs>? TotalPriceChanged;
    event EventHandler<EventArgs>? TotalItemsChanged;

    ReadOnlyObservableCollection<CartItem> Items { get; }
    decimal TotalPrice { get; }
    int TotalItems { get; }

    void AddToCart(PizzaItem pizzaItem, int count);
    void RemoveFromCart(CartItem pizzaItem);

    void Decrement(int wareId, int decrement);
    void Increment(int wareId, int decrement);
}

public class CartService : ICartService
{
    private readonly ObservableCollection<CartItem> _items = new();
    private readonly IWaresService _waresService;

    public event EventHandler<EventArgs>? TotalPriceChanged;
    public event EventHandler<EventArgs>? TotalItemsChanged;

    public CartService(IWaresService waresService)
    {
        _waresService = waresService;
        Items = new ReadOnlyObservableCollection<CartItem>(_items);
    }

    public ReadOnlyObservableCollection<CartItem> Items { get; }
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }

    public void AddToCart(PizzaItem pizzaItem, int count)
    {
        var match = Items.FirstOrDefault(x => x.Id == pizzaItem.Id);
        if (match != null)
        {
            match.Count = count;
        }
        else
        {
            _items.Add(new CartItem
            {
                Id = pizzaItem.Id,
                Image = pizzaItem.Image,
                Name = pizzaItem.Name,
                Price = pizzaItem.Price,
                Count = count,
                IsLiked = pizzaItem.IsLiked,
                Description = pizzaItem.Description,
            });
        }

        TotalItems = _items.Sum(x => x.Count);
        TotalPrice = _items.Sum(x => x.TotalPrice);
        TotalPriceChanged?.Invoke(this, EventArgs.Empty);
        TotalItemsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveFromCart(CartItem pizzaItem) 
    {
        _items.Remove(pizzaItem);
        TotalItems = _items.Sum(x => x.Count);
        TotalPrice = _items.Sum(x => x.TotalPrice);
        TotalPriceChanged?.Invoke(this, EventArgs.Empty);
        TotalItemsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Decrement(int wareId, int decrement)
    {
        var item = _items.FirstOrDefault(x => x.Id == wareId);
        if (item == null)
            return;

        int newValue = item.Count - decrement;
        if (newValue == 0)
        {
            _items.Remove(item);
        }
        else
        {
            item.Count = newValue;
        }

        TotalItems = _items.Sum(x => x.Count);
        TotalPrice = _items.Sum(x => x.TotalPrice);
        TotalPriceChanged?.Invoke(this, EventArgs.Empty);
        TotalItemsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Increment(int wareId, int increment)
    {
        var item = _items.FirstOrDefault(x => x.Id == wareId);
        if (item == null)
        {
            var ware = _waresService.GetAllWares().FirstOrDefault(item => item.Id == wareId);
            if (ware == null)
                return;

            item = new CartItem
            {
                Id = wareId,
                Image = ware.Image,
                Name = ware.Name,
                Price = ware.Price,
                Description = ware.Description,
                IsLiked = ware.IsLiked,
            };
            _items.Add(item);
        }

        item.Count += increment;
        TotalItems = _items.Sum(x => x.Count);
        TotalPrice = _items.Sum(x => x.TotalPrice);
        TotalPriceChanged?.Invoke(this, EventArgs.Empty);
        TotalItemsChanged?.Invoke(this, EventArgs.Empty);
    }
}
