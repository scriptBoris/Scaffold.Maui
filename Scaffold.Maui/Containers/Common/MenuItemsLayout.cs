using Microsoft.Maui.Layouts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.Common;

public class MenuItemsLayout : Layout, ILayoutManager, IDisposable
{
    private List<IView>? _visibleItems;
    private IView? _menuView;

    // items source
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IList<ScaffoldMenuItem>),
        typeof(MenuItemsLayout),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItemsLayout self)
            {
                if (o is INotifyCollectionChanged old)
                    old.CollectionChanged -= self.Notify_CollectionChanged;

                self.TryUpdate();
            }
        }
    );
    public IList<ScaffoldMenuItem>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty) as IList<ScaffoldMenuItem>;
        set => SetValue(ItemsSourceProperty, value);
    }

    // max visible items
    public static readonly BindableProperty MaxVisibleItemsProperty = BindableProperty.Create(
        nameof(MaxVisibleItems),
        typeof(int),
        typeof(MenuItemsLayout),
        2,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItemsLayout self)
            {
                self.TryUpdate();
            }
        }
    );
    public int MaxVisibleItems
    {
        get => (int)GetValue(MaxVisibleItemsProperty);
        set => SetValue(MaxVisibleItemsProperty, value);
    }

    // items spacing
    public static readonly BindableProperty ItemsSpacingProperty = BindableProperty.Create(
        nameof(ItemsSpacing),
        typeof(double),
        typeof(MenuItemsLayout),
        0.0,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItemsLayout self)
            {
                self.InvalidateMeasure();
            }
        }
    );
    public double ItemsSpacing
    {
        get => (double)GetValue(ItemsSpacingProperty);
        set => SetValue(ItemsSpacingProperty, value);
    }

    // item template
    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(MenuItemsLayout),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItemsLayout self)
            {
                self.TryUpdate();
            }
        }
    );
    public DataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty) as DataTemplate;
        set => SetValue(ItemTemplateProperty, value);
    }

    // menu button template
    public static readonly BindableProperty MenuButtonTemplateProperty = BindableProperty.Create(
        nameof(MenuButtonTemplate),
        typeof(DataTemplate),
        typeof(MenuItemsLayout),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is MenuItemsLayout self)
            {
                self.TryUpdate();
            }
        }
    );
    public DataTemplate? MenuButtonTemplate
    {
        get => GetValue(MenuButtonTemplateProperty) as DataTemplate;
        set => SetValue(MenuButtonTemplateProperty, value);
    }

    private void TryUpdate()
    {
        if (ItemsSource == null || ItemTemplate == null || MenuButtonTemplate == null ||
            ItemsSource.Count == 0)
            return;

        Children.Clear();
        _visibleItems = new(MaxVisibleItems);

        if (ItemsSource is INotifyCollectionChanged notify)
        {
            notify.CollectionChanged += Notify_CollectionChanged;
        }

        int realVisibleItems = 0;
        int potencialVisibleItems = 0;
        int i = -1;
        foreach (var item in ItemsSource)
        {
            i++;

            if (!item.IsVisible || item.IsCollapsed)
                continue;

            if (realVisibleItems < MaxVisibleItems)
            {
                var itemView = (View)ItemTemplate.CreateContent();
                itemView.BindingContext = ItemsSource[i];
                _visibleItems.Add(itemView);
                Children.Add(itemView);
                realVisibleItems++;
            }

            // TODO Реализвать обработку на IsVisible и IsCollapsed у каждого элемента колекции
            potencialVisibleItems++;
        }

        if (potencialVisibleItems > MaxVisibleItems)
        {
            var menuView = (View)MenuButtonTemplate.CreateContent();
            menuView.BindingContext = BindingContext;
            _menuView = menuView;
            Children.Add(menuView);
        }
        else
        {
            _menuView = null;
        }

        InvalidateMeasure();
    }

    private void Notify_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not IList list)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var added = (ScaffoldMenuItem)e.NewItems![0]!;
                int addedId = e.NewStartingIndex;

                // ignore
                if (_visibleItems != null && _visibleItems.Count >= MaxVisibleItems)
                    return;

                if (!added.IsVisible || added.IsCollapsed)
                    return;

                if (_visibleItems == null)
                    _visibleItems = new List<IView>(MaxVisibleItems);

                // Вставляем элемент в правильном порядке
                int i = 0;
                bool isAdded = false;
                foreach (var item in _visibleItems)
                {
                    var binding = ((Element)item).BindingContext;
                    int index = list.IndexOf(binding);

                    if (addedId < index)
                    {
                        InsertItemView(added, i);
                        isAdded = true;
                        break;
                    }
                    i++;
                }

                if (!isAdded)
                    InsertItemView(added, _visibleItems.Count);

                break;
            case NotifyCollectionChangedAction.Remove:
                // TODO Реализовать удаление элементов в колекции
                //int oldDelId = e.OldStartingIndex;
                //var oldDel = e.OldItems![0]!;
                //var visibleDelete = _visibleItems?.LastOrDefault(x => ((Element)x).BindingContext == oldDel);
                //if (visibleDelete == null)
                //    return;
                throw new NotImplementedException();
                break;
            case NotifyCollectionChangedAction.Replace:
                // TODO реализовать замену элементов в колекции
                throw new NotImplementedException();
                break;
            case NotifyCollectionChangedAction.Move:
                // TODO реализовать перемещение элементов по колекции
                throw new NotImplementedException();
                break;
            case NotifyCollectionChangedAction.Reset:
                Children.Clear();
                _visibleItems = null;
                _menuView = null;
                break;
            default:
                break;
        }
    }

    private void InsertItemView(object context, int i)
    {
        var view = (View)ItemTemplate.CreateContent();
        view.BindingContext = context;
        _visibleItems.Insert(i, view);
        Children.Add(view);
    }

    public Size ArrangeChildren(Rect bounds)
    {
        double x = 0;
        if (_visibleItems != null && _visibleItems.Count > 0)
        {
            foreach (var item in _visibleItems)
            {
                double height = item.DesiredSize.Height;
                double width = item.DesiredSize.Width;
                double y = bounds.Height / 2 - height / 2;

                var b = new Rect(x, y, width, height);
                item.Arrange(b);

                x += width + ItemsSpacing;
            }
        }

        if (_menuView != null)
        {
            double height = _menuView.DesiredSize.Height;
            double width = _menuView.DesiredSize.Width;
            double y = bounds.Height / 2 - height / 2;
            var b = new Rect(x, y, width, height);
            _menuView.Arrange(b);
        }
        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        double freeW = widthConstraint;
        double maxH = 0;
        double maxW = 0;
        int totalItems = 0;

        if (_visibleItems != null && _visibleItems.Count > 0)
        {
            foreach (var item in _visibleItems)
            {
                var size = item.Measure(freeW, heightConstraint);
                maxH = Math.Max(maxH, size.Height);
                maxW += size.Width;
            }
            totalItems += _visibleItems.Count;
        }

        if (_menuView != null)
        {
            var size = _menuView.Measure(freeW, heightConstraint);
            maxH = Math.Max(maxH, size.Height);
            maxW += size.Width;
            totalItems++;
        }

        if (totalItems > 2)
            maxW += ItemsSpacing * (totalItems - 1);

        return new Size(maxW, maxH);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    public void Dispose()
    {
        if (ItemsSource is INotifyCollectionChanged n)
            n.CollectionChanged -= Notify_CollectionChanged;
    }
}
