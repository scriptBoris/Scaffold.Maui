using SampleDll.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDll.Models;

public class PizzaItem : BaseNotify
{
    public required int Id { get; set; }
    public required ImageSource Image { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }

    public bool IsLiked { get; set; }
}
