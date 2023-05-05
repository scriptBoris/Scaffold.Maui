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
        VisibleItems = new();
        CollapsedItems = new();
    }

    internal ObservableCollection<MenuItem> VisibleItems { get; private set; }
    internal ObservableCollection<MenuItem> CollapsedItems { get; private set; }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Reset:
                if (e.OldItems != null)
                {
                    int index = e.OldStartingIndex;
                    var item = e.OldItems[index] as MenuItem;
                    item?.SetupParent(null);
                }
                break;
            default:
                break;
        }

        foreach (var item in Items)
        {
            item.SetupParent(this);
            item.BindingContext = attachedView.BindingContext;
        }

        Update();
    }

    internal void Update()
    {
        VisibleItems.Clear();
        CollapsedItems.Clear();

        foreach (var item in Items)
        {
            if (!item.IsVisible)
                continue;

            if (item.Mode == MenuItemModes.Default && VisibleItems.Count < 3)
            {
                VisibleItems.Add(item);
            }
            else
            {
                CollapsedItems.Add(item);
            }
        }
    }
}
