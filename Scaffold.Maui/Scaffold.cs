using ButtonSam.Maui;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.LifecycleEvents;
using Scaffold.Maui.Containers;
using Scaffold.Maui.Core;
using Scaffold.Maui.Internal;
using System.Collections.ObjectModel;

namespace Scaffold.Maui;

public interface IScaffold
{
    public const int AlertIndexZ = 999;

    Task PushAsync(View view, bool isAnimating = true);
    Task PopAsync(bool isAnimated = true);
    Task PopToRootAsync(bool isAnimated = true);

    Task DisplayAlert(string title, string message, string cancel);
    Task<bool> DisplayAlert(string title, string message, string ok, string cancel);
}

public class ScaffoldView : Layout, IScaffold, ILayoutManager, IDisposable
{
    public const ushort AnimationTime = 180;

    public ScaffoldView()
    {
        NavigationContainer = new();
        ZBufer = new();

#if ANDROID
        NavigationBar = new Platforms.Android.NavigationBar(this);
#endif

        Children.Add(NavigationBar);
        Children.Add(NavigationContainer);
        Children.Add(ZBufer);
    }

    #region bindable props
    // vc context
    public static readonly BindableProperty ScaffoldContextProperty = BindableProperty.CreateAttached(
        "ScaffoldContext",
        typeof(IScaffold),
        typeof(ScaffoldView),
        null
    );
    internal static void SetScaffoldContext(BindableObject b, IScaffold value)
    {
        b.SetValue(ScaffoldContextProperty, value);
    }
    public static IScaffold? GetScaffoldContext(BindableObject b)
    {
        return b.GetValue(ScaffoldContextProperty) as IScaffold;
    }

    // title
    public static readonly BindableProperty TitleProperty = BindableProperty.CreateAttached(
        "ScaffoldTitle",
        typeof(string),
        typeof(ScaffoldView),
        null
    );
    public static void SetTitle(BindableObject b, string? value)
    {
        b.SetValue(TitleProperty, value);
    }
    public static string? GetTitle(BindableObject b)
    {
        return b.GetValue(TitleProperty) as string;
    }

    // has navigation bar
    public static readonly BindableProperty HasNavigationBarProperty = BindableProperty.CreateAttached(
        "ScaffoldHasNavigationBar",
        typeof(bool),
        typeof(ScaffoldView),
        true
    );
    public static void SetHasNavigationBar(BindableObject b, bool value)
    {
        b.SetValue(HasNavigationBarProperty, value);
    }
    public static bool GetHasNavigationBar(BindableObject b)
    {
        return (bool)b.GetValue(HasNavigationBarProperty);
    }

    // menu items
    public static readonly BindableProperty MenuItemsProperty = BindableProperty.CreateAttached(
        "ScaffoldMenuItems",
        typeof(MenuItemObs),
        typeof(ScaffoldView),
        null,
        defaultValueCreator: b =>
        {
            var obs = new MenuItemObs(b);
            return obs;
        }
    );
    public static MenuItemObs GetMenuItems(BindableObject b)
    {
        return (MenuItemObs)b.GetValue(MenuItemsProperty);
    }
    #endregion bindable props

    public INavigationBar NavigationBar { get; private set; }
    public NavigationContainer NavigationContainer { get; private set; }
    public ZBuffer ZBufer { get; private set; }

    internal void SoftwareBackButtonInternal()
    {
        this.Dispatcher.Dispatch(SoftwareBackButton);
    }

    protected virtual void SoftwareBackButton()
    {
        PopAsync(true);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    public Size ArrangeChildren(Rect bounds)
    {
        double x = 0;
        double y = 0;
        double w = bounds.Width;

        var barRect = new Rect(x, y, w, NavigationBar.DesiredSize.Height);
        ((IView)NavigationBar).Arrange(barRect);

        var cRect = new Rect(x, NavigationBar.DesiredSize.Height, w, NavigationContainer.DesiredSize.Height);
        ((IView)NavigationContainer).Arrange(cRect);

        var zRect = new Rect(x, y, w, bounds.Height);
        ((IView)ZBufer).Arrange(zRect);

        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        double freeHeight = heightConstraint;
        var bar = ((IView)NavigationBar).Measure(widthConstraint, heightConstraint);
        freeHeight -= bar.Height;

        ((IView)NavigationContainer).Measure(widthConstraint, freeHeight);
        ((IView)ZBufer).Measure(widthConstraint, heightConstraint);

        return new Size(widthConstraint, heightConstraint);
    }

    public Task InsertView(View view, int index, bool isAnimated = true)
    {
        // TODO Доработать InsertView!
        return NavigationContainer.InserAsync(view, index, isAnimated);
    }

    public Task PushAsync(View view, bool isAnimated = true)
    {
        ScaffoldView.SetScaffoldContext(view, this);

        bool hasBackButton = NavigationContainer.NavigationStack.Count > 0;

        return Task.WhenAll(
            NavigationContainer.PushAsync(view, isAnimated),
            NavigationBar.SwitchContent(new NavigationSwitchArgs
            {
                ActionType = IntentType.Push,
                NewContent = view,
                IsAnimating = isAnimated,
                HasBackButton = hasBackButton,
            })
        );
    }

    public Task PopAsync(bool isAnimated = true)
    {
        int count = NavigationContainer.NavigationStack.Count;
        bool hasBackButton = count > 2;
        var prev = NavigationContainer.NavigationStack.ItemOrDefault(count - 2);

        if (prev == null)
            return Task.CompletedTask;

        return Task.WhenAll(
            NavigationContainer.PopAsync(isAnimated),
            NavigationBar.SwitchContent(new NavigationSwitchArgs
            {
                ActionType = IntentType.Pop,
                NewContent = prev,
                IsAnimating = isAnimated,
                HasBackButton = hasBackButton,
            })
        );
    }

    public Task PopToRootAsync(bool isAnimated = true)
    {
        int count = NavigationContainer.NavigationStack.Count;
        var prev = NavigationContainer.NavigationStack.FirstOrDefault();

        if (prev == null || count == 1)
            return Task.CompletedTask;

        return Task.WhenAll(
            NavigationContainer.PopToRoot(isAnimated),
            NavigationBar.SwitchContent(new NavigationSwitchArgs
            {
                ActionType = IntentType.Pop,
                NewContent = prev,
                IsAnimating = isAnimated,
                HasBackButton = false,
            })
        );
    }

    public async Task DisplayAlert(string title, string message, string cancel)
    {
        var alert = new DisplayAlertLayer(title, message, cancel);
        ZBufer.AddLayer(alert, IScaffold.AlertIndexZ);
        await alert.GetResult();
    }

    public async Task<bool> DisplayAlert(string title, string message, string ok, string cancel)
    {
        var alert = new DisplayAlertLayer(title, message, ok, cancel);
        ZBufer.AddLayer(alert, IScaffold.AlertIndexZ);
        return await alert.GetResult();
    }

    public void Dispose()
    {
        NavigationBar.Dispose();
        NavigationContainer.Dispose();
        ZBufer.Dispose();
    }
}

public static class ScaffoldExtensions
{
    public static IScaffold? GetContext(this View view)
    {
        return ScaffoldView.GetScaffoldContext(view);
    }
}