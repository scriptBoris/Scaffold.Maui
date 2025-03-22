using Microsoft.Maui.Layouts;
using ScaffoldLib.Maui.Internal;
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
    private CacheController? _cacheController;
    private IView? _menuView;
    private int _collapsedItems;

    #region bindable props
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
                // desubscribe
                if (o is INotifyCollectionChanged old)
                    old.CollectionChanged -= self.Notify_CollectionChanged;

                if (o is IList<ScaffoldMenuItem> oldList)
                    foreach (var item in oldList)
                    {
                        item.SortIndex = -1;
                        item.AdvancedPropertyChanged -= self.ItemPropChanged;
                    }

                self.TryUpdate();

                if (n is IList<ScaffoldMenuItem> newList)
                {
                    int i = 0;
                    foreach (var item in newList)
                    {
                        item.SortIndex = i;
                        item.AdvancedPropertyChanged += self.ItemPropChanged;
                        i++;
                    }
                }
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
    #endregion bindable props

    public IEnumerable<Rect> UndragAreas
    {
        get
        {
            if (_menuView is View menuView && menuView.IsVisible)
                yield return menuView.AbsRect();

            if (_cacheController != null)
            foreach (var item in _cacheController.Views)
            {
                if (item is View vitem)
                    yield return vitem.AbsRect();
            }

            yield break;
        }
    }

    private void TryUpdate()
    {
        if (ItemsSource == null || ItemTemplate == null || MenuButtonTemplate == null)
            return;

        if (_cacheController == null)
        {
            _cacheController = new(MaxVisibleItems, this);
        }
        else
        {
            _cacheController.Reset();
        }

        if (ItemsSource is INotifyCollectionChanged notify)
        {
            notify.CollectionChanged += Notify_CollectionChanged;
        }

        int fullVisibleItems = 0;
        int potencialVisibleItems = 0;
        int collapsedItems = 0;
        for (int i = 0; i < ItemsSource.Count; i++)
        {
            var item = ItemsSource[i];
            var cache = _cacheController[i];

            if (cache == null)
                cache = _cacheController.ResolveAdd(item, i, ItemTemplate);

            if (!item.IsVisible)
            {
                cache.Hide();
                continue;
            }

            if (item.IsCollapsed)
            {
                collapsedItems++;
                potencialVisibleItems++;
                cache.Hide();
                continue;
            }

            if (fullVisibleItems < MaxVisibleItems)
            {
                cache.Show();
                fullVisibleItems++;
            }
            else
            {
                cache.Hide();
            }

            potencialVisibleItems++;
        }

        _collapsedItems = collapsedItems;
        _cacheController.RecalcCache(fullVisibleItems);

        bool showMenuButton = potencialVisibleItems > MaxVisibleItems || _collapsedItems > 0;
        ResolveMenuView(showMenuButton);
        InvalidateMeasure();
    }

    private void SoftUpdate()
    {
        int fullVisibleItems = 0;
        int potencialVisibleItems = 0;
        int collapsedItems = 0;

        for (int i = 0; i < ItemsSource!.Count; i++)
        {
            var item = ItemsSource[i];
            var cache = _cacheController[i];

            if (!item.IsVisible)
            {
                continue;
            }

            if (item.IsCollapsed)
            {
                collapsedItems++;
                potencialVisibleItems++;
                continue;
            }

            if (fullVisibleItems < MaxVisibleItems)
            {
                fullVisibleItems++;
            }

            potencialVisibleItems++;
        }

        _collapsedItems = collapsedItems;
        bool showMenuButton = potencialVisibleItems > MaxVisibleItems || collapsedItems > 0;
        ResolveMenuView(showMenuButton);
    }

    private void Notify_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not IList list || _cacheController == null || ItemTemplate == null)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    var logicItem = (ScaffoldMenuItem)e.NewItems![0]!;
                    int addedId = e.NewStartingIndex;

                    // защита от глупого hotreload, в случае когда в IDE 
                    // закоментировать целиком <Scaffold.MenuItems> ... </Scaffold.MenuItems>
                    // тогда ничего не делаем
                    if (_cacheController.CheckExists(logicItem))
                        return;

                    logicItem.AdvancedPropertyChanged += ItemPropChanged;

                    var item = _cacheController.ResolveAdd(logicItem, addedId, ItemTemplate);

                    bool show = logicItem.IsVisible && !logicItem.IsCollapsed;
                    if (show)
                    {
                        item.Show();
                    }
                    else
                    {
                        item.Hide();
                    }

                    if (logicItem.IsVisible && logicItem.IsCollapsed)
                        _collapsedItems++;

                    ResolveMenuView();
                    this.InvalidateMeasure();
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                {
                    int index = e.OldStartingIndex;
                    var logicItem = (ScaffoldMenuItem)e.OldItems![0]!;
                    logicItem.AdvancedPropertyChanged -= ItemPropChanged;

                    var item = _cacheController[index];
                    item.Hide();
                    _cacheController.Remove(item);

                    SoftUpdate();
                    this.InvalidateMeasure();
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                SoftUpdate();
                this.InvalidateMeasure();
                break;
            case NotifyCollectionChangedAction.Move:
                SoftUpdate();
                this.InvalidateMeasure();
                break;
            case NotifyCollectionChangedAction.Reset:
                for (int i = _cacheController.TotalItems - 1; i >= 0; i--)
                {
                    var item = _cacheController[i];
                    _cacheController.Remove(item);
                    item.MenuItem.AdvancedPropertyChanged -= ItemPropChanged;
                }
                TryUpdate();
                break;
            default:
                break;
        }
    }

    private void ItemPropChanged(object? sender, ScaffoldMenuItem.ChangedArgs e)
    {
        if (_cacheController == null || ItemsSource == null)
            return;

        var item = (ScaffoldMenuItem)sender!;
        int index = ItemsSource.IndexOf(item);
        switch (e.Type)
        {
            case ScaffoldMenuItem.PropertyTypes.IsVisible:
                {
                    bool isHide = !item.IsVisible || item.IsCollapsed;
                    if (isHide)
                    {
                        _cacheController[index].Hide();
                    }
                    else
                    {
                        _cacheController[index].Show();
                    }

                    // TODO улучшить алгоритм перерисовки элементов. Убрать избыточные вычисления
                    SoftUpdate();
                    this.InvalidateMeasure();
                }
                break;
            case ScaffoldMenuItem.PropertyTypes.IsCollapsed:
                {
                    bool isHide = !item.IsVisible || item.IsCollapsed;
                    if (isHide)
                    {
                        _cacheController[index].Hide();
                    }
                    else
                    {
                        _cacheController[index].Show();
                    }

                    if (item.IsCollapsed && item.IsVisible)
                    {
                        _collapsedItems++;
                    }
                    else
                    {
                        _collapsedItems--;
                    }

                    ResolveMenuView();
                    this.InvalidateMeasure();
                }
                break;
            default:
                break;
        }
    }

    private void ResolveMenuView(bool? show = null)
    {
        bool flag;
        if (show == null)
            flag = _cacheController.IsOverflowed || _collapsedItems > 0;
        else
            flag = show.Value;

        if (flag)
        {
            if (_menuView == null)
            {
                var menuView = (View)MenuButtonTemplate!.CreateContent();
                _menuView = menuView;
                Children.Add(menuView);
            }
            else
            {
                if (_menuView is View m)
                    m.IsVisible = true;
            }
        }
        else
        {
            if (_menuView is View m)
                m.IsVisible = false;
        }
    }

    public Size ArrangeChildren(Rect bounds)
    {
        double x = 0;
        if (_cacheController != null)
        {
            foreach (var item in _cacheController.Views)
            {
                double height = item.DesiredSize.Height;
                double width = item.DesiredSize.Width;
                double y = bounds.Height / 2 - height / 2;

                var b = new Rect(x, y, width, height);
                item.Arrange(b);

                x += width + ItemsSpacing;
            }
        }

        if (_menuView != null && _menuView.Visibility == Visibility.Visible)
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

        if (_cacheController != null)
        {
            foreach (var item in _cacheController.Views)
            {
                var size = item.Measure(freeW, heightConstraint);
                maxH = Math.Max(maxH, size.Height);
                maxW += size.Width;
            }
            totalItems += _cacheController.VisibleItemsCount;
        }

        if (_menuView != null && _menuView.Visibility == Visibility.Visible)
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

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        var context = BindingContext;

        if (ItemsSource != null)
            foreach (var item in ItemsSource)
            {
                item.BindingContext = context;
            }
    }

    public void Dispose()
    {
        if (ItemsSource is INotifyCollectionChanged n)
            n.CollectionChanged -= Notify_CollectionChanged;
    }

    private class CacheController : IComparer<InternalItem>
    {
        private readonly MenuItemsLayout _host;
        private readonly List<InternalItem> _logicItems;
        private readonly List<IView> _freePool;

        /// <summary>
        /// Актуальна ли сортировка <see cref="_visibleItems"/>
        /// </summary>
        private bool _isActualSortedViews;

        /// <summary>
        /// Элементы которые хотят быть видимыми, т.е. все элементы котоные 
        /// имеют флаги:
        /// <br/>
        /// IsCollapsed = false
        /// <br/>
        /// IsVisible = true
        /// </summary>
        private readonly List<InternalItem> _visibleItems;

        /// <summary>
        /// Пул views
        /// </summary>
        private readonly List<View> _totalViewPool;

        public CacheController(int capacity, MenuItemsLayout host)
        {
            _host = host;
            _logicItems = new(capacity);
            _freePool = new(capacity);

            _totalViewPool = new(capacity);
            _visibleItems = new(capacity);
        }

        /// <summary>
        /// Кол-во видимых элементов
        /// </summary>
        public int VisibleItemsCount
        {
            get
            {
                if (_visibleItems.Count > _host.MaxVisibleItems)
                    return _host.MaxVisibleItems;

                return _visibleItems.Count;
            }
        }

        public int TotalItems => _logicItems.Count;

        /// <summary>
        /// Флаг обозначающий что есть элементы которые не могут поместиться на панели
        /// </summary>
        public bool IsOverflowed => _visibleItems.Count > _host.MaxVisibleItems;

        /// <summary>
        /// Отсортированные элементы views, готовые для отрисовки
        /// </summary>
        public IEnumerable<IView> Views
        {
            get
            {
                if (!_isActualSortedViews)
                {
                    _visibleItems.Sort(this);
                    RecalcCache();
                    _isActualSortedViews = true;
                }

                int i = 0;
                foreach (var item in _visibleItems)
                {
                    if (i < _host.MaxVisibleItems)
                        yield return item.View;
                    else
                        yield break;

                    i++;
                }

                yield break;
            }
        }

        public InternalItem this[int index]
        {
            get
            {
                if (index < 0 || index > _logicItems.Count - 1)
                    return null;

                return _logicItems[index];
            }
            set => _logicItems[index] = value;
        }

        public bool CheckExists(ScaffoldMenuItem context)
        {
            return context.Parent != null;
        }

        public InternalItem ResolveAdd(ScaffoldMenuItem context, int originIndex, DataTemplate template)
        {
            IView view;
            if (_freePool.Count > 0)
            {
                var free = (View)_freePool.First();
                _freePool.Remove(free);
                view = free;
            }
            else
            {
                var v = (View)template.CreateContent();
                view = v;
                _host.Children.Add(view);
                _totalViewPool.Add(v);
            }

            context.Parent = this;

            var internalItem = new InternalItem
            {
                Parent = this,
                MenuItem = context,
                View = view,
                SortIndex = originIndex,
            };
            _logicItems.Add(internalItem);

            return internalItem;
        }

        public void Remove(InternalItem item)
        {
            _logicItems.Remove(item);
            item.MenuItem.Parent = null;
        }

        public bool Hide(InternalItem internalItem)
        {
            if (!internalItem.IsDisplayed)
                return false;

            _visibleItems.Remove(internalItem);
            internalItem.IsDisplayed = false;

            if (internalItem.View is View v)
                v.IsVisible = false;

            _isActualSortedViews = false;
            return true;
        }

        public bool Show(InternalItem internalItem)
        {
            if (internalItem.IsDisplayed)
                return false;

            _visibleItems.Add(internalItem);
            internalItem.IsDisplayed = true;
            _isActualSortedViews = false;
            return true;
        }

        public void Reset()
        {
            _freePool.Clear();
            _visibleItems.Clear();

            for (int i = _totalViewPool.Count - 1; i >= 0; i--)
            {
                var del = _totalViewPool[i];
                del.IsVisible = false;
                del.BindingContext = null;

                _freePool.Add(del);
            }
        }

        public void RecalcCache(int? fullVisibleItems = null)
        {
            int limit = _totalViewPool.Count;
            int visItems = Math.Min(_visibleItems.Count, _host.MaxVisibleItems);

            for (int i = 0; i < limit; i++)
            {
                var view = _totalViewPool[i];
                if (i <= visItems - 1)
                {
                    view.BindingContext = _visibleItems[i].MenuItem;
                    view.IsVisible = true;
                    _visibleItems[i].View = view;
                }
                else
                {
                    view.IsVisible = false;
                }
            }
        }

        public int Compare(InternalItem x, InternalItem y)
        {
            return x.SortIndex - y.SortIndex;
        }
    }

    private class InternalItem
    {
        public required CacheController Parent { get; set; }
        public int SortIndex { get; set; }
        public required IView? View { get; set; }
        public required ScaffoldMenuItem MenuItem { get; set; }
        public bool IsDisplayed { get; set; }

        internal void Arrange(Rect b)
        {
            View?.Arrange(b);
        }

        internal Size Measure(double freeW, double heightConstraint)
        {
            return View?.Measure(freeW, heightConstraint) ?? new Size();
        }

        internal bool Hide()
        {
            return Parent.Hide(this);
        }

        internal bool Show()
        {
            return Parent.Show(this);
        }
    }
}