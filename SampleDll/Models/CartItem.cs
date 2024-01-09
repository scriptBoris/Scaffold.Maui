using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDll.Models
{
    public class CartItem : PizzaItem
    {
        public int Count { get; set; }

        [DependsOn(nameof(Count), nameof(Price))]
        public decimal TotalPrice => Price * Count;
    }
}
