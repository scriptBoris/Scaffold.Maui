using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers
{
    public interface IViewWrapper
    {
        View? Overlay { get; set; }
        View View { get; }
        Task UpdateVisual(NavigatingArgs args);
    }
}
