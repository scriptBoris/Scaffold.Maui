using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui;

public class ScaffoldMenuItems : ObservableCollection<ScaffoldMenuItem>
{
    internal BindableObject? BindableObject { get; set; }

    protected override void InsertItem(int index, ScaffoldMenuItem item)
    {
        item.BindingContext = BindableObject;
        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, ScaffoldMenuItem item)
    {
        item.BindingContext = BindableObject;
        base.SetItem(index, item);
    }
}