using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core;

public class MenuItemCollection : ObservableCollection<MenuItem>, IDisposable
{
    private readonly BindableObject _attachedView;

    public MenuItemCollection(BindableObject attachedView)
    {
        _attachedView = attachedView;
        _attachedView.BindingContextChanged += AttachedView_BindingContextChanged;

        foreach (var item in this)
            item.BindingContext = attachedView.BindingContext;

        VisibleItems = new();
        CollapsedItems = new();
    }

    private void AttachedView_BindingContextChanged(object? sender, EventArgs e)
    {
        foreach (var item in this)
            item.BindingContext = _attachedView.BindingContext;
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
            item.BindingContext = _attachedView.BindingContext;
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

            if (!item.IsCollapsed && VisibleItems.Count < 3)
            {
                VisibleItems.Add(item);
            }
            else
            {
                CollapsedItems.Add(item);
            }
        }
    }

    public void Dispose()
    {
        _attachedView.BindingContextChanged -= AttachedView_BindingContextChanged;
    }
}
