using Sample.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Models;

public class PizzaItem : BaseNotify
{
    public required ImageSource Image { get; set; }
    public required string Name { get; set; }
    public required double Price { get; set; }

    public bool IsLiked { get; set; }
}
