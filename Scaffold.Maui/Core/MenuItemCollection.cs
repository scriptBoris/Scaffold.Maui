using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core;

public class MenuItemCollection : ObservableCollection<MenuItem>, IDisposable
{
    private readonly BindableObject _attachedView;
    private const int maxVis = 2;

    public MenuItemCollection(BindableObject attachedView)
    {
        _attachedView = attachedView;
        _attachedView.BindingContextChanged += AttachedView_BindingContextChanged;

        VisibleItems = new();
        CollapsedItems = new();

        foreach (var item in this)
        {
            item.SetParent(this);
            item.BindingContext = attachedView.BindingContext;
            ResolveItem(item);
        }
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
            case NotifyCollectionChangedAction.Add:
                var add = (MenuItem)e.NewItems![0]!;
                add.SetParent(this);
                add.BindingContext = _attachedView.BindingContext;
                ResolveItem(add);
                break;
            case NotifyCollectionChangedAction.Remove:
                var rm = (MenuItem)e.OldItems![0]!;
                CollapsedItems.Remove(rm);
                VisibleItems.Remove(rm);
                rm.SetParent(null);
                break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var item in this)
                {
                    item.SetParent(null);
                    VisibleItems.Clear();
                    CollapsedItems.Clear();
                }
                break;
            default:
                break;
        }
    }

    internal void ResolveItem(MenuItem item)
    {
        if (!item.IsVisible)
        {
            CollapsedItems.Remove(item);
            VisibleItems.Remove(item);
            return;
        }

        if (item.IsCollapsed)
        {
            VisibleItems.Remove(item);
            int id = MenuItemCollection.FindPos(item, this, CollapsedItems);
            if (id >= 0)
                CollapsedItems.Insert(id, item);
        }
        else
        {
            CollapsedItems.Remove(item);
            int id = MenuItemCollection.FindPos(item, this, VisibleItems);
            if (id >= 0)
                VisibleItems.Insert(id, item);
        }

        CheckOverflow();
    }

    internal void CheckOverflow()
    {
        bool isForceCollapsed = VisibleItems.Count > maxVis;
        if (isForceCollapsed)
        {
            var last = VisibleItems.Last();
            VisibleItems.Remove(last);
            int id = MenuItemCollection.FindPos(last, this, CollapsedItems);
            if (id >= 0)
                CollapsedItems.Insert(id, last);
        }
    }

    // TODO В будущем побороть этот дурацкий алгоритм
    internal void ResolveItem(MenuItem item, bool oldVisible, bool oldCollapse)
    {
        VisibleItems.Clear();
        CollapsedItems.Clear();

        foreach (var i in this)
        {
            ResolveItem(i);
        }
    }

    private static int FindPos(MenuItem item, IList<MenuItem> master, IList<MenuItem> slave)
    {
        int value = master.IndexOf(item);
        if (slave.IndexOf(item) >= 0)
            return -1;

        int left = -1;
        for (int i = 0; i < slave.Count; i++)
        {
            if (i > 0)
                left = master.IndexOf(slave[i - 1]);

            int right = master.IndexOf(slave[i]);

            if (left < value && value < right)
                return i;
        }

        return slave.Count;
    }

    public void Dispose()
    {
        _attachedView.BindingContextChanged -= AttachedView_BindingContextChanged;
    }
}
