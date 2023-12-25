using SamplePizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePizza.Services;

public interface IWaresService
{
    List<PizzaItem> GetAllWares();
}

public class WaresService : IWaresService
{
    public List<PizzaItem> GetAllWares()
    {
        var wares = new List<PizzaItem>
        {
            new()
            {
                Id = 1,
                Image = "pizza_1.png",
                Name = "Mixsik",
                Description = "Бекон, цыпленок, ветчина, сыр блю чиз, сыры чеддер и пармезан, соус песто, кубики брынзы, томаты, красный лук, моцарелла, фирменный соус альфредо, чеснок, итальянские травы",
                Price = 10.29m,
            },
            new() 
            {
                Id = 2,
                Image = "pizza_2.png",
                Name = "Chirozo fresh",
                Description = "Острая чоризо, сладкий перец, моцарелла, фирменный томатный соус",
                Price = 12.79m,
            },
            new()
            {
                Id = 3,
                Image = "pizza_3.png",
                Name = "Burger pizza",
                Description = "Ветчина, маринованные огурчики, томаты, красный лук, чеснок, соус бургер, моцарелла, фирменный томатный соус",
                Price = 13.50m,
            },
            new()
            {
                Id = 4,
                Image = "pizza_4.png",
                Name = "Pepperoni fresh",
                Price = 9.90m,
                Description = "Пикантная пепперони, увеличенная порция моцареллы, томаты, фирменный томатный соус",
            },
            new()
            {
                Id = 5,
                Image = "pizza_5.png",
                Name = "Cheese chicken",
                Price = 14.90m,
                Description = "Цыпленок, моцарелла, фирменный соус альфредо",
            },
            new()
            {
                Id = 6,
                Image = "pizza_6.png",
                Name = "Ham and cheese",
                Price = 9.90m,
                Description = "Ветчина, моцарелла, фирменный соус альфредо",
            },
            new()
            {
                Id = 7,
                Image = "pizza_7.png",
                Name = "Сheese",
                Price = 8.90m,
                Description = "Моцарелла, сыры чеддер и пармезан, фирменный соус альфредо",
            },
            new()
            {
                Id = 8,
                Image = "pizza_8.png",
                Name = "Double chicken",
                Price = 16.50m,
                Description = "Цыпленок, моцарелла, фирменный соус альфредо",
            },
        };
        return wares;
    }
}
