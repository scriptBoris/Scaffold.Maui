using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Containers;
using System.Threading.Channels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Animations;
using ScaffoldLib.Maui.Args;
#if IOS
using UIKit;
#endif

namespace ScaffoldLib.Maui;

public interface IScaffold : IScaffoldProvider, IWindowsBehavior
{
    public const int MenuItemsIndexZ = 998;
    public const int AlertIndexZ = 999;
    public const int ToastIndexZ = 997;

    ReadOnlyObservableCollection<IBehavior> ExternalBevahiors { get; }
    ReadOnlyObservableCollection<View> NavigationStack { get; }
    ViewFactory ViewFactory { get; }

    Task PushAsync(View view, bool isAnimating = true);
    Task<bool> PopAsync(bool isAnimated = true);
    Task<bool> PopToRootAsync(bool isAnimated = true);
    Task<bool> PopToRootAndSetRootAsync(View newRootView, bool isAnimated = true);
    Task<bool> RemoveView(View view, bool isAnimated = true);
    Task<bool> ReplaceView(View oldView, View newView, bool isAnimated = true);
    Task<bool> InsertView(View view, int index, bool isAnimated = true);

    Task<bool> DisplayAlert(CreateDisplayAlertArgs args);
    Task<bool> DisplayAlert(CreateDisplayAlertArgs args, View parentView);

    Task<IDisplayActionSheetResult> DisplayActionSheet(CreateDisplayActionSheet args);
    Task<IDisplayActionSheetResult> DisplayActionSheet(CreateDisplayActionSheet args, View parentView);
    
    Task Toast(CreateToastArgs args);
    void AddCustomLayer(IZBufferLayout layer, int zIndex);
    void AddCustomLayer(IZBufferLayout layer, int zIndex, View parentView);
}

public class Scaffold : Layout, IScaffold, ILayoutManager, IDisposable, IBackButtonListener, IAppear, IDisappear, IRemovedFromNavigation
{
    public const ushort AnimationTime = 180;

    private readonly ObservableCollection<IBehavior> _externalBevahiors = new();
    private readonly NavigationController _navigationController;
    private readonly ZBuffer _zBufer;
    private IBackButtonBehavior? _backButtonBehavior;
    private static Thickness _deviceSafeArea;
    private Thickness _safeArea;

    public static event EventHandler<Thickness>? DeviceSafeAreaChanged;

    internal static void Preserve()
    {
    }

    static Scaffold()
    {
#if ANDROID
        PlatformSpec = new ScaffoldLib.Maui.Platforms.Android.PlatformSpecific();
#elif WINDOWS
        PlatformSpec = new ScaffoldLib.Maui.Platforms.Windows.PlatformSpecific();
#elif IOS
        PlatformSpec = new ScaffoldLib.Maui.Platforms.iOS.PlatformSpecific();
#endif
    }

    public Scaffold()
    {
        if (!Initializer.IsInitialized)
        {
            const string error = "ScaffoldLib.Maui.Scaffold not initialized. Please use UseScaffold() in MauiProgram.cs";
            Console.WriteLine(error);
            throw new Exception(error);
        }

        _safeArea = DeviceSafeArea;
        BindingContext = null;
        ExternalBevahiors = new(_externalBevahiors);
        ExternalBevahiors.AsNotifyObs().CollectionChanged += Scaffold_CollectionChanged; ;

        _navigationController = new(this);
        _zBufer = new();
        Children.Add(_zBufer);

        DeviceSafeAreaChanged += OnSafeAreaChanged;
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
        propertyChanged: (b, o, n) =>
        {
            if (GetScaffoldContext(b) is Scaffold scaffold)
                scaffold
                    ._navigationController
                    .Agents
                    .LastOrDefault(x => x.ViewWrapper.View == b)?
                    .NavigationBar?
                    .UpdateTitle(n as string);
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
        propertyChanged: (b, o, n) =>
        {
            var agent = FindAgent(b);
            if (agent != null)
                agent.HasNavigationBar = (bool)n;
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
        propertyChanged: (b, o, n) =>
        {
            if (b is Scaffold scaffold && scaffold._navigationController != null)
            {
                foreach (var agent in scaffold._navigationController.Agents)
                {
                    var color = GetNavigationBarBackgroundColor(agent.ViewWrapper.View) ?? n as Color ?? DefaultNavigationBarBackgroundColor;
                    agent.NavigationBarBackgroundColor = color;
                    agent.ResolveStatusBarColor();
                }
            }
            else
            {
                var agent = FindAgent(b);
                if (agent == null)
                    return;

                var context = (Scaffold)agent.Context;
                var color = n as Color ?? context.NavigationBarBackgroundColor ?? DefaultNavigationBarBackgroundColor;
                agent.NavigationBarBackgroundColor = color;
                agent.ResolveStatusBarColor();
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
                foreach (var agent in scaffold._navigationController.Agents)
                {
                    var color = GetNavigationBarForegroundColor(agent.ViewWrapper.View) ?? n as Color ?? DefaultNavigationBarForegroundColor;
                    agent.NavigationBar?.UpdateNavigationBarForegroundColor(color);
                }
            }
            else
            {
                var agent = FindAgent(b);
                if (agent == null)
                    return;

                var context = (Scaffold)agent.Context;
                var color = n as Color ?? context.NavigationBarForegroundColor ?? DefaultNavigationBarForegroundColor;
                agent.NavigationBarForegroundColor = color;
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
        propertyChanged: (b, o, n) =>
        {
            if (GetScaffoldContext(b) is Scaffold context)
            {
                context
                    ._navigationController
                    .Agents
                    .LastOrDefault(x => x.ViewWrapper.View == b)?
                    .NavigationBar?
                    .UpdateMenuItems(n as Core.MenuItemCollection);
            }
        }
    );
    public static Core.MenuItemCollection GetMenuItems(BindableObject b)
    {
        return (Core.MenuItemCollection)b.GetValue(MenuItemsProperty);
    }

    // status bar foreground color
    public static readonly BindableProperty StatusBarForegroundColorProperty = BindableProperty.CreateAttached(
        "StatusBarForegroundColor",
        typeof(StatusBarColorTypes),
        typeof(Scaffold),
        StatusBarColorTypes.DependsByNavigationBarColor,
        propertyChanged: (b, o, n) =>
        {
            if (GetScaffoldContext(b) is Scaffold context)
                context
                    ._navigationController
                    .Agents
                    .LastOrDefault(x => x.ViewWrapper.View == b && x.IsAppear)?
                    .ResolveStatusBarColor();
        }
    );
    public static void SetStatusBarForegroundColor(BindableObject b, StatusBarColorTypes value) =>
        b.SetValue(StatusBarForegroundColorProperty, value);
    public static StatusBarColorTypes GetStatusBarForegroundColor(BindableObject b) =>
        (StatusBarColorTypes)b.GetValue(StatusBarForegroundColorProperty);

    // is content under navigation bar
    public static readonly BindableProperty IsContentUnderNavigationBarProperty = BindableProperty.CreateAttached(
        "IsContentUnderNavigationBar",
        typeof(bool),
        typeof(Scaffold),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (GetScaffoldContext(b) is Scaffold context)
            {
                var agent = context
                    ._navigationController
                    .Agents
                    .LastOrDefault(x => x.ViewWrapper.View == b && x.IsAppear);
                if (agent is IView v)
                    v.InvalidateMeasure();
            }
        }
    );
    public static void SetIsContentUnderNavigationBar(BindableObject b, bool value) =>
        b.SetValue(IsContentUnderNavigationBarProperty, value);
    public static bool GetIsContentUnderNavigationBar(BindableObject b) =>
        (bool)b.GetValue(IsContentUnderNavigationBarProperty);

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
    internal static IPlatformSpecific PlatformSpec { get; private set; }

    internal AppearingStates AppearingState { get; private set; } = AppearingStates.None;

    public static Thickness DeviceSafeArea
    {
        get => _deviceSafeArea;
        internal set
        {
            if (_deviceSafeArea != value)
            {
                _deviceSafeArea = value;
                DeviceSafeAreaChanged?.Invoke(null, value);
            }
        }
    }

    public Thickness SafeArea
    {
        get => _safeArea;
        internal set
        {
            if (_safeArea != value)
            {
                _safeArea = value;

                if (_navigationController != null)
                    foreach (var frame in _navigationController.Agents)
                        frame.SafeArea = value;
            }
        }
    }

    public Rect[] UndragArea
    {
        get
        {
            var list = new List<Rect>();
            if (CurrentAgent is IWindowsBehavior windowsBehavior)
                list.AddRange(windowsBehavior.UndragArea);

            if (ProvideScaffold is IWindowsBehavior sw)
                list.AddRange(sw.UndragArea);

            return list.ToArray();
        }
    }

    public ReadOnlyObservableCollection<IBehavior> ExternalBevahiors { get; }
    public ReadOnlyObservableCollection<View> NavigationStack => _navigationController.NavigationStack;

    public IScaffold? ProvideScaffold
    {
        get
        {
            if (_navigationController.CurrentAgent?.ViewWrapper.View is IScaffold sc)
                return sc;

            if (_navigationController.CurrentAgent?.ViewWrapper.View is IScaffoldProvider provider)
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
            foreach (var agent in _navigationController.Agents)
                agent.BackButtonBehavior = value;
        }
    }

    internal ZBuffer ZBuffer => _zBufer;
    internal IAgent? CurrentAgent => _navigationController.CurrentAgent;
    internal static Color DefaultNavigationBarBackgroundColor => Color.FromArgb("#6200EE");
    internal static Color DefaultNavigationBarForegroundColor => Colors.White;

    private IBackButtonListener BackButtonListener
    {
        get
        {
            var last = _navigationController.CurrentAgent?.ViewWrapper.View;
            if (last is IBackButtonListener v)
                return v;
            else if (last?.BindingContext is IBackButtonListener vm)
                return vm;

            return this;
        }
    }
    #endregion props

    private void Scaffold_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in _navigationController.Agents)
                    item.OnBehaiorAdded((IBehavior)e.NewItems![0]!);
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in _navigationController.Agents)
                    item.OnBehaiorRemoved((IBehavior)e.OldItems![0]!);
                break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var item in _navigationController.Agents)
                    foreach (IBehavior behavior in e.OldItems!)
                        item.OnBehaiorRemoved(behavior);
                break;
            default:
                break;
        }
    }

    private void OnSafeAreaChanged(object? sender, Thickness e)
    {
        SafeArea = e;
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
        var current = _navigationController.CurrentAgent?.ViewWrapper.View;
        if (current == null)
            return true;

        if (_navigationController.CurrentAgent!.ZBuffer.Pop())
            return true;

        bool canPop = await BackButtonListener.OnBackButton();
        if (canPop)
            return await RemoveView(current, true);
        else
            return true;
    }

    private bool isAddedDebug;
    internal void TryDrawDebugLabel()
    {
#if RELEASE
        return;
#endif

        if (isAddedDebug)
            return;

        isAddedDebug = true;
        ZBuffer.AddLayer(new Containers.Common.DebugInfo(), 888);
    }

    public void AddBehavior(IBehavior behavior)
    {
        _externalBevahiors.Add(behavior);
    }

    public bool RemoveBehavior(IBehavior behavior)
    {
        return _externalBevahiors.Remove(behavior);
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
        if (_navigationController.CurrentAgent is IView frame)
            frame.Arrange(bounds);

        ((IView)_zBufer).Arrange(bounds);

        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        // new
        if (_navigationController.CurrentAgent is IView frame)
            frame.Measure(widthConstraint, heightConstraint);

        ((IView)_zBufer).Measure(widthConstraint, heightConstraint);

        return new Size(widthConstraint, heightConstraint);
    }

    public async Task PushAsync(View view, bool isAnimated = true)
    {
        Scaffold.SetScaffoldContext(view, this);
        await _navigationController.PushAsync(view, isAnimated);
    }

    public async Task<bool> InsertView(View view, int index, bool isAnimated = true)
    {
        if (index < 0)
            return false;

        int count = NavigationStack.Count;
        if (count == 0 || index >= count)
            await PushAsync(view, isAnimated);

        Scaffold.SetScaffoldContext(view, this);
        _navigationController.InsertView(view, index);
        return true;
    }

    public async Task<bool> ReplaceView(View oldView, View newView, bool isAnimated = true)
    {
        var oldIndex = NavigationStack.IndexOf(oldView);
        if (oldIndex < 0)
            return false;

        if (NavigationStack.Count == 0)
            return false;

        if (oldIndex == NavigationStack.Count - 1)
        {
            Scaffold.SetScaffoldContext(newView, this);
            await _navigationController.ReplaceView(newView, isAnimated);
            return true;
        }
        else
        {
            bool success = await InsertView(newView, oldIndex, isAnimated);
            if (success)
                await RemoveView(oldView, false);

            return success;
        }
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

    public async Task<bool> PopToRootAndSetRootAsync(View newRootView, bool isAnimated = true)
    {
        int count = _navigationController.NavigationStack.Count;
        if (count == 0)
            return false;

        var oldView = _navigationController.NavigationStack.Last();
        if (count == 1)
            return await ReplaceView(oldView, newRootView, isAnimated);

        for (int i = count - 2; i >= 0; i--)
        {
            _navigationController.RemoveView(i);
        }

        return await ReplaceView(oldView, newRootView, isAnimated);
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

    public void AddCustomLayer(IZBufferLayout layer, int zIndex)
    {
        _zBufer.AddLayer(layer, zIndex);
    }

    public void AddCustomLayer(IZBufferLayout layer, int zIndex, View parentView)
    {
        var agent = FindAgent(parentView);
        if (agent == null)
            return;

        agent.ZBuffer.AddLayer(layer, zIndex);
    }

    public async Task<bool> DisplayAlert(CreateDisplayAlertArgs args)
    {
        var alert = ViewFactory.CreateDisplayAlert(args);
        _zBufer.AddLayer(alert, IScaffold.AlertIndexZ);
        return await alert.GetResult();
    }

    public Task<bool> DisplayAlert(CreateDisplayAlertArgs args, View parentView)
    {
        var agent = FindAgent(parentView);
        if (agent == null)
            return Task.FromResult(false);

        var alert = ViewFactory.CreateDisplayAlert(args);
        agent.ZBuffer.AddLayer(alert, IScaffold.AlertIndexZ);
        return alert.GetResult();
    }

    public Task<IDisplayActionSheetResult> DisplayActionSheet(CreateDisplayActionSheet args)
    {
        var alert = ViewFactory.CreateDisplayActionSheet(args);
        _zBufer.AddLayer(alert, IScaffold.AlertIndexZ);
        return alert.GetResult();
    }

    public Task<IDisplayActionSheetResult> DisplayActionSheet(CreateDisplayActionSheet args, View parentView)
    {
        var agent = FindAgent(parentView);
        if (agent == null)
            return Task.FromResult<IDisplayActionSheetResult>(new DisplayActionSheetResult
            {
                IsCanceled = true,
            });

        var alert = ViewFactory.CreateDisplayActionSheet(args);
        agent.ZBuffer.AddLayer(alert, IScaffold.AlertIndexZ);
        return alert.GetResult();
    }

    public Task Toast(CreateToastArgs args)
    {
        var toast = ViewFactory.CreateToast(args);
        if (toast == null)
            return Task.CompletedTask;

        _zBufer.AddLayer(toast, IScaffold.ToastIndexZ);
        return toast.GetResult();
    }

    public void OnAppear(bool isComplete)
    {
        var stl = AppearingState;
        AppearingState = isComplete ? AppearingStates.Appear : AppearingStates.Appearing;
        if (stl == AppearingState)
            return;

        var f = _navigationController.Agents.LastOrDefault();
        if (f != null)
        {
            var view = f.ViewWrapper.View;
            var bgColor = Scaffold.GetNavigationBarBackgroundColor(view) ?? NavigationBarBackgroundColor ?? DefaultNavigationBarBackgroundColor;
            f.TryAppearing(isComplete, AppearingState, bgColor);
        }
    }

    public void OnDisappear(bool isComplete)
    {
        var stl = AppearingState;
        AppearingState = isComplete ? AppearingStates.Disappear : AppearingStates.Disappearing;
        if (stl == AppearingState)
            return;

        var f = _navigationController.Agents.LastOrDefault();
        f?.TryDisappearing(isComplete, stl);
    }

    public void OnRemovedFromNavigation()
    {
        foreach (var frame in _navigationController.Agents.Reverse())
            frame.TryRemoveFromNavigation();
    }

    public void Dispose()
    {
        OnRemovedFromNavigation();
        DeviceSafeAreaChanged -= OnSafeAreaChanged;
        _navigationController.Dispose();
        ExternalBevahiors.AsNotifyObs().CollectionChanged -= Scaffold_CollectionChanged;
        _zBufer.Dispose();
    }

    public static void SetupStatusBarColor(StatusBarColorTypes colorType)
    {
        PlatformSpec.SetStatusBarColorScheme(colorType);
    }

    public static void TryHideKeyboard()
    {
#if ANDROID
        var context = Platform.AppContext;
        if (context.GetSystemService(Android.Content.Context.InputMethodService) is Android.Views.InputMethods.InputMethodManager inputMethodManager)
        {
            var activity = Platform.CurrentActivity;
            var token = activity?.CurrentFocus?.WindowToken;
            inputMethodManager.HideSoftInputFromWindow(token, Android.Views.InputMethods.HideSoftInputFlags.None);
            activity?.Window?.DecorView.ClearFocus();
        }
#elif IOS
        var window = UIKit.UIApplication.SharedApplication?.KeyWindow;
        var rootView = window?.RootViewController?.View;

        if (rootView != null)
        {
            rootView.EndEditing(true);
        }
#endif
    }

    public static IAgent? FindAgent(BindableObject bindable)
    {
        if (Scaffold.GetScaffoldContext(bindable) is not Scaffold context)
            return null;

        return context._navigationController.Agents.FirstOrDefault(x => x.ViewWrapper.View == bindable);
    }

    public static IScaffold? GetRootScaffold()
    {
        return Application.Current?.MainPage?.GetRootScaffold();
    }
}

public static class ScaffoldExtensions
{
    public static IScaffold? GetContext(this View view)
    {
        return Scaffold.GetScaffoldContext(view);
    }

    public static View? GetPage(this View view)
    {
        if (view.GetContext() != null)
            return view;

        View? parent = view.Parent as View;
        while (true)
        {
            parent = parent?.Parent as View;
            if (parent?.GetContext() != null)
            {
                break;
            }
        }

        return parent;
    }
}