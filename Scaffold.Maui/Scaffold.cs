using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui;

public interface IScaffold : IScaffoldProvider
{
    public const int MenuItemsIndexZ = 998;
    public const int AlertIndexZ = 999;

    Thickness SafeArea { get; }
    ReadOnlyObservableCollection<View> NavigationStack { get; }

    Task PushAsync(View view, bool isAnimating = true);
    Task<bool> PopAsync(bool isAnimated = true);
    Task<bool> PopToRootAsync(bool isAnimated = true);
    Task<bool> RemoveView(View view, bool isAnimated = true);
    Task<bool> ReplaceView(View oldView, View newView, bool isAnimated = true);
    Task<bool> InsertView(View view, int index, bool isAnimated = true);

    Task DisplayAlert(string title, string message, string cancel);
    Task<bool> DisplayAlert(string title, string message, string ok, string cancel);
}

public class Scaffold : Layout, IScaffold, ILayoutManager, IDisposable, IBackButtonListener, IAppear, IDisappear, IRemovedFromNavigation
{
    public const ushort AnimationTime = 180;
    
    private readonly NavigationController _navigationController;
    private readonly ZBuffer _zBufer;
    private IBackButtonBehavior? _backButtonBehavior;

    public Scaffold()
    {
#if ANDROID
        SafeArea = global::ScaffoldLib.Maui.Platforms.Android.ScaffoldAndroid.GetSafeArea();
#endif
        _navigationController = new(this);
        ((INotifyCollectionChanged)_navigationController.Frames).CollectionChanged += FramesStackChanged;

        _zBufer = new();
        Children.Add(_zBufer);
    }

    #region bindable props
    // scaffold context
    public static readonly BindableProperty ScaffoldContextProperty = BindableProperty.CreateAttached(
        "ScaffoldContext",
        typeof(IScaffold),
        typeof(Scaffold),
        null
    );
    internal static void SetScaffoldContext(BindableObject b, IScaffold value) =>
        b.SetValue(ScaffoldContextProperty, value);
    public static IScaffold? GetScaffoldContext(BindableObject b) =>
        b.GetValue(ScaffoldContextProperty) as IScaffold;

    // title
    public static readonly BindableProperty TitleProperty = BindableProperty.CreateAttached(
        "ScaffoldTitle",
        typeof(string),
        typeof(Scaffold),
        null,
        propertyChanged:(b,o,n) =>
        {
            if (GetScaffoldContext(b) is Scaffold scaffold)
            {
                var navBar = scaffold
                    ._navigationController
                    .Frames
                    .FirstOrDefault(x => x.ViewWrapper.View == b)?
                    .NavigationBar;

                if (navBar != null)
                    navBar.Title = n as string;
                //.UpdateTitle(n as string);
            }
        }
    );
    public static void SetTitle(BindableObject b, string? value) => 
        b.SetValue(TitleProperty, value);
    public static string? GetTitle(BindableObject b) => 
        b.GetValue(TitleProperty) as string;

    // has navigation bar
    public static readonly BindableProperty HasNavigationBarProperty = BindableProperty.CreateAttached(
        "ScaffoldHasNavigationBar",
        typeof(bool),
        typeof(Scaffold),
        true,
        propertyChanged:(b,o,n) =>
        {
            if (GetScaffoldContext(b) is Scaffold scaffold)
            {
                scaffold
                    ._navigationController
                    .Frames
                    .FirstOrDefault(x => x.ViewWrapper.View == b)?
                    .DrawLayout();
                    //.NavigationBar?
                    //.UpdateNavigationBarVisible((bool)n);
            }
        }
    );
    public static void SetHasNavigationBar(BindableObject b, bool value) =>
        b.SetValue(HasNavigationBarProperty, value);
    public static bool GetHasNavigationBar(BindableObject b) =>
        (bool)b.GetValue(HasNavigationBarProperty);

    // navigation bar background color
    public static readonly BindableProperty NavigationBarBackgroundColorProperty = BindableProperty.CreateAttached(
        "NavigationBarBackgroundColor",
        typeof(Color),
        typeof(Scaffold),
        null,
        propertyChanged:(b,o,n) =>
        {
            if (b is Scaffold scaffold && scaffold._navigationController != null)
            {
                foreach (var frame in scaffold._navigationController.Frames)
                {
                    var color = GetNavigationBarBackgroundColor(frame.ViewWrapper.View) ?? n as Color ?? defaultNavigationBarBackgroundColor;
                    frame.NavigationBar?.UpdateNavigationBarBackgroundColor(color);
                }
            }
            else if (GetScaffoldContext(b) is Scaffold context)
            {
                var color = n as Color ?? context.NavigationBarBackgroundColor ?? defaultNavigationBarBackgroundColor;
                context
                    ._navigationController
                    .Frames
                    .FirstOrDefault(x => x.ViewWrapper.View == b)?
                    .NavigationBar?
                    .UpdateNavigationBarBackgroundColor(color);
            }
        }
    );
    public static void SetNavigationBarBackgroundColor(BindableObject b, Color? value) =>
        b.SetValue(NavigationBarBackgroundColorProperty, value);
    public static Color? GetNavigationBarBackgroundColor(BindableObject b) =>
        b.GetValue(NavigationBarBackgroundColorProperty) as Color;
    public Color? NavigationBarBackgroundColor
    {
        get => GetValue(NavigationBarBackgroundColorProperty) as Color;
        set => SetValue(NavigationBarBackgroundColorProperty, value);
    }

    // navigation bar foreground color
    public static readonly BindableProperty NavigationBarForegroundColorProperty = BindableProperty.CreateAttached(
        "NavigationBarForegroundColor",
        typeof(Color),
        typeof(Scaffold),
        //Colors.White,
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is Scaffold scaffold && scaffold._navigationController != null)
            {
                foreach (var frame in scaffold._navigationController.Frames)
                {
                    var color = GetNavigationBarForegroundColor(frame.ViewWrapper.View) ?? n as Color ?? defaultNavigationBarForegroundColor;
                    frame.NavigationBar?.UpdateNavigationBarForegroundColor(color);
                }
            }
            else if (GetScaffoldContext(b) is Scaffold context)
            {
                var color = n as Color ?? context.NavigationBarForegroundColor ?? defaultNavigationBarForegroundColor;
                context
                    ._navigationController
                    .Frames
                    .FirstOrDefault(x => x.ViewWrapper.View == b)?
                    .NavigationBar?
                    .UpdateNavigationBarForegroundColor(color);
            }
        }
    );
    public static void SetNavigationBarForegroundColor(BindableObject b, Color? value) =>
        b.SetValue(NavigationBarForegroundColorProperty, value);
    public static Color? GetNavigationBarForegroundColor(BindableObject b) =>
        b.GetValue(NavigationBarForegroundColorProperty) as Color;
    public Color? NavigationBarForegroundColor
    {
        get => GetValue(NavigationBarForegroundColorProperty) as Color;
        set => SetValue(NavigationBarForegroundColorProperty, value);
    }

    // menu items
    public static readonly BindableProperty MenuItemsProperty = BindableProperty.CreateAttached(
        "ScaffoldMenuItems",
        typeof(Core.MenuItemCollection),
        typeof(Scaffold),
        null,
        defaultValueCreator: b =>
        {
            return new Core.MenuItemCollection(b);
        },
        propertyChanged: (b,o,n) =>
        {
            if (GetScaffoldContext(b) is Scaffold context)
            {
                context
                    ._navigationController
                    .Frames
                    .FirstOrDefault(x => x.ViewWrapper.View == b)?
                    .NavigationBar?
                    .UpdateMenuItems((View)b);
            }
        }
    );
    public static Core.MenuItemCollection GetMenuItems(BindableObject b)
    {
        return (Core.MenuItemCollection)b.GetValue(MenuItemsProperty);
    }

    // view factory
    public static readonly BindableProperty ViewFactoryProperty = BindableProperty.Create(
        nameof(ViewFactory),
        typeof(ViewFactory),
        typeof(Scaffold),
        new ViewFactory()
    );
    public ViewFactory ViewFactory
    {
        get => (ViewFactory)GetValue(ViewFactoryProperty);  
        set => SetValue(ViewFactoryProperty, value);
    }
    #endregion bindable props

    #region props
    public ReadOnlyObservableCollection<View> NavigationStack => _navigationController.NavigationStack;
    public Thickness SafeArea { get; private set; }
    public IScaffold? ProvideScaffold
    {
        get 
        {
            if (_navigationController.CurrentFrame?.ViewWrapper.View is IScaffold sc)
                return sc;

            if (_navigationController.CurrentFrame?.ViewWrapper.View is IScaffoldProvider provider)
                return provider.ProvideScaffold;

            return null;
        }
    }

    public IBackButtonBehavior? BackButtonBehavior 
    {
        get => _backButtonBehavior;
        set
        {
            _backButtonBehavior = value;
            foreach (var item in _navigationController.Frames)
                item.NavigationBar?.UpdateBackButtonBehavior(value);
        } 
    }

    internal ZBuffer ZBuffer => _zBufer;
    internal static Color defaultNavigationBarBackgroundColor => Color.FromArgb("#6200EE");
    internal static Color defaultNavigationBarForegroundColor => Colors.White;

    private IBackButtonListener BackButtonListener
    {
        get 
        {
            var last = _navigationController.CurrentFrame?.ViewWrapper.View;
            if (last is IBackButtonListener v)
                return v;
            else if (last?.BindingContext is IBackButtonListener vm)
                return vm;

            return this; 
        }
    }
    #endregion props

    private void FramesStackChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
                var frame = e.OldItems![0] as Containers.IFrame;

                frame?.TryRemoveFromNavigation();

                if (frame is IDisposable dis)
                    dis.Dispose();

                break;
            default:
                break;
        }
    }

    internal IScaffold[] GetScafoldNested()
    {
        var list = new List<IScaffold> { this };
        var scaffold = this as IScaffold;

        while (scaffold != null)
        {
            scaffold = scaffold is IScaffoldProvider provider ? provider.ProvideScaffold : null;
            if (scaffold != null)
                list.Add(scaffold);
        }

        return list.ToArray();
    }

    internal async Task<bool> HardwareBackButtonInternal()
    {
        var current = _navigationController.CurrentFrame?.ViewWrapper.View;
        if (current == null)
            return true;

        bool canPop = await BackButtonListener.OnBackButton();
        if (canPop)
            return await RemoveView(current, true);
        else
            return false;
    }

    internal void SoftwareBackButtonInternal()
    {
        this.Dispatcher.Dispatch(async () => {
            bool canPop = await BackButtonListener.OnBackButton();
            if (canPop)
                await PopAsync().ConfigureAwait(false);
        });
    }

    internal void ShowCollapsedMenusInternal(View view)
    {
        this.Dispatcher.Dispatch(() => {
            var overlay = ViewFactory.CreateDisplayMenuItemslayer(view);
            _zBufer.AddLayer(overlay, IScaffold.MenuItemsIndexZ);
        });
    }

    public virtual Task<bool> OnBackButton()
    {
        return Task.FromResult(true);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    public Size ArrangeChildren(Rect bounds)
    {
        if (_navigationController.CurrentFrame is IView frame)
            frame.Arrange(bounds);

        ((IView)_zBufer).Arrange(bounds);

        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        // new
        if (_navigationController.CurrentFrame is IView frame)
            frame.Measure(widthConstraint, heightConstraint);

        ((IView)_zBufer).Measure(widthConstraint, heightConstraint);

        return new Size(widthConstraint, heightConstraint);
    }

    public async Task PushAsync(View view, bool isAnimated = true)
    {
        Scaffold.SetScaffoldContext(view, this);
        await _navigationController.PushAsync(view, isAnimated);
    }

    public Task<bool> PopAsync(bool isAnimated = true)
    {
        _zBufer.RemoveLayerAsync(IScaffold.MenuItemsIndexZ).ConfigureAwait(true);
        return _navigationController.PopAsync(isAnimated);
    }

    public async Task<bool> PopToRootAsync(bool isAnimated = true)
    {
        int count = _navigationController.NavigationStack.Count;
        if (count <= 1)
            return false;

        if (count == 2)
            await PopAsync(isAnimated);

        for (int i = count - 2; i > 0; i--)
        {
            _navigationController.RemoveView(i);
        }

        return await PopAsync(isAnimated);
    }

    public async Task<bool> RemoveView(View view, bool isAnimated)
    {
        int count = _navigationController.NavigationStack.Count;
        if (count <= 1)
            return false;

        int index = _navigationController.NavigationStack.IndexOf(view);
        if (index < 0)
            return false;

        if (index == _navigationController.NavigationStack.Count - 1)
        {
            return await PopAsync(isAnimated);
        }
        else
        {
            return _navigationController.RemoveView(index);
        }
    }

    public async Task<bool> ReplaceView(View oldView, View newView, bool isAnimated = true)
    {
        if (_navigationController.NavigationStack.Count == 0)
            return false;

        return await _navigationController.ReplaceAsync(oldView, newView, isAnimated);
    }

    public Task<bool> InsertView(View view, int index, bool isAnimated = true)
    {
        return _navigationController.InsertView(view, index, isAnimated);
    }

    public async Task DisplayAlert(string title, string message, string cancel)
    {
        var alert = ViewFactory.CreateDisplayAlert(title, message, cancel);
        _zBufer.AddLayer(alert, IScaffold.AlertIndexZ);
        await alert.GetResult();
    }

    public async Task<bool> DisplayAlert(string title, string message, string ok, string cancel)
    {
        var alert = ViewFactory.CreateDisplayAlert(title, message, ok, cancel);
        _zBufer.AddLayer(alert, IScaffold.AlertIndexZ);
        return await alert.GetResult();
    }

    public void OnAppear(bool isComplete)
    {
        if (_navigationController.NavigationStack.LastOrDefault() is IAppear ap)
            ap.OnAppear(isComplete);
    }

    public void OnDisappear(bool isComplete)
    {
        if (_navigationController.NavigationStack.LastOrDefault() is IDisappear ap)
            ap.OnDisappear(isComplete);
    }

    public void OnRemovedFromNavigation()
    {
        foreach (var frame in _navigationController.Frames.Reverse())
            frame.TryRemoveFromNavigation();
    }

    public void Dispose()
    {
        OnRemovedFromNavigation();
        ((INotifyCollectionChanged)_navigationController.Frames).CollectionChanged -= FramesStackChanged;
        _navigationController.Dispose();
        _zBufer.Dispose();
    }
}

public static class ScaffoldExtensions
{
    public static IScaffold? GetContext(this View view)
    {
        return Scaffold.GetScaffoldContext(view);
    }
}