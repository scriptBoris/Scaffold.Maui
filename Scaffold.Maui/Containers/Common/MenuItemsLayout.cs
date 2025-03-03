using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.Common;

public class MenuItemsLayout : Layout, ILayoutManager
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

        int forLimit = Math.Min(MaxVisibleItems, ItemsSource.Count);
        for (int i = 0; i < forLimit; i++)
        {
            var itemView = (View)ItemTemplate.CreateContent();
            itemView.BindingContext = ItemsSource[i];
            _visibleItems.Add(itemView);
            Children.Add(itemView);
        }

        if (ItemsSource.Count > MaxVisibleItems)
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
}
