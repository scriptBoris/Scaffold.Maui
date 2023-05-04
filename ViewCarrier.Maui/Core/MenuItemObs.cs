using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core;

public class MenuItemObs : ObservableCollection<MenuItem>
{
    private readonly BindableObject attachedView;

    public MenuItemObs(BindableObject attachedView)
    {
        this.attachedView = attachedView;
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        foreach (var item in Items)
        {
            item.BindingContext = attachedView.BindingContext;
        }
    }
}
