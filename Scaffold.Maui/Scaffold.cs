﻿using ButtonSam.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.LifecycleEvents;
using Scaffold.Maui.Containers;
using Scaffold.Maui.Core;
using Scaffold.Maui.Internal;
using System.Collections.ObjectModel;

namespace Scaffold.Maui;

public interface IScaffold
{
    public const int MenuIndexZ = 998;
    public const int AlertIndexZ = 999;

    ReadOnlyObservableCollection<View> NavigationStack { get; }

    Task PushAsync(View view, bool isAnimating = true);
    Task<bool> PopAsync(bool isAnimated = true);
    Task<bool> PopToRootAsync(bool isAnimated = true);
    Task<bool> RemoveView(View view, bool isAnimated = true);
    Task<bool> ReplaceView(View oldView, View newView, bool isAnimated = true);

    Task DisplayAlert(string title, string message, string cancel);
    Task<bool> DisplayAlert(string title, string message, string ok, string cancel);
}

public class ScaffoldView : Layout, IScaffold, ILayoutManager, IDisposable, IBackButtonListener, IScaffoldProvider
{
    public const ushort AnimationTime = 180;
    private readonly NavigationController _navigationController;
    private readonly ZBuffer _zBufer;

    public ScaffoldView()
    {
        _navigationController = new(this);
        _zBufer = new();
        Children.Add(_zBufer);
    }

    #region bindable props
    // scaffold context
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
        null,
        propertyChanged:(b,o,n) =>
        {
            if (GetScaffoldContext(b) is ScaffoldView scaffold)
            {
                scaffold._navigationController
                .Frames
                .FirstOrDefault(x => x.View == b)?
                .UpdateTitle(n as string);
            }
        }
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
        true,
        propertyChanged:(b,o,n) =>
        {
            if (GetScaffoldContext(b) is ScaffoldView scaffold)
            {
                scaffold._navigationController
                .Frames
                .FirstOrDefault(x => x.View == b)?
                .UpdateNavigationBarVisible((bool)n);
            }
        }
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
            return new MenuItemObs(b);
        },
        propertyChanged: (b,o,n) =>
        {
            if (GetScaffoldContext(b) is ScaffoldView context)
            {
                context._navigationController
                .Frames
                .FirstOrDefault(x => x.View == b)?
                .NavigationBar?
                .UpdateMenu((View)b);
            }
        }
    );
    public static MenuItemObs GetMenuItems(BindableObject b)
    {
        return (MenuItemObs)b.GetValue(MenuItemsProperty);
    }
    #endregion bindable props

    public ReadOnlyObservableCollection<View> NavigationStack => _navigationController.NavigationStack;
    public IScaffold? ProvideScaffold => _navigationController.CurrentFrame?.View as IScaffold;
    private IBackButtonListener BackButtonListener => _navigationController.CurrentFrame?.View as IBackButtonListener ?? this;

    internal int ImmestionLength()
    {
        int count = 0;
        var scaffold = this as IScaffold;

        while (scaffold != null)
        {
            count += Math.Max(scaffold.NavigationStack.Count - 1, 0);
            scaffold = scaffold is IScaffoldProvider provider ? provider.ProvideScaffold : null;
        }

        return count;
    }

    internal async Task<bool> HardwareBackButtonInternal()
    {
        bool successAlert = await _zBufer.RemoveLayerAsync(IScaffold.MenuIndexZ);
        if (successAlert)
            return false;

        bool canPop = await BackButtonListener.OnBackButton();
        if (canPop)
        {
            await PopAsync().ConfigureAwait(false);
            return true;
        }
        else
        {
            return false;
        }
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
        this.Dispatcher.Dispatch(() => OnShowCollapsedMenus(view));
    }

    public virtual Task<bool> OnBackButton()
    {
        return Task.FromResult(true);
    }

    protected virtual void OnShowCollapsedMenus(View view)
    {
        var overlay = new DisplayMenuItemslayer(view);
        _zBufer.AddLayer(overlay, IScaffold.MenuIndexZ);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    public Size ArrangeChildren(Rect bounds)
    {
        foreach (var item in _navigationController.Frames)
            ((IView)item).Arrange(bounds);

        ((IView)_zBufer).Arrange(bounds);

        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        foreach (var item in _navigationController.Frames)
            ((IView)item).Measure(widthConstraint, heightConstraint);

        ((IView)_zBufer).Measure(widthConstraint, heightConstraint);

        return new Size(widthConstraint, heightConstraint);
    }

    public async Task PushAsync(View view, bool isAnimated = true)
    {
        ScaffoldView.SetScaffoldContext(view, this);
        await _navigationController.PushAsync(view, isAnimated);
    }

    public Task<bool> PopAsync(bool isAnimated = true)
    {
        _zBufer.RemoveLayerAsync(IScaffold.MenuIndexZ).ConfigureAwait(true);
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

    public async Task DisplayAlert(string title, string message, string cancel)
    {
        var alert = new DisplayAlertLayer(title, message, cancel);
        _zBufer.AddLayer(alert, IScaffold.AlertIndexZ);
        await alert.GetResult();
    }

    public async Task<bool> DisplayAlert(string title, string message, string ok, string cancel)
    {
        var alert = new DisplayAlertLayer(title, message, ok, cancel);
        _zBufer.AddLayer(alert, IScaffold.AlertIndexZ);
        return await alert.GetResult();
    }

    public void Dispose()
    {
        foreach (var item in Children)
        {
            if (item is IDisposable disposable)
                disposable.Dispose();
        }
    }
}

public static class ScaffoldExtensions
{
    public static IScaffold? GetContext(this View view)
    {
        return ScaffoldView.GetScaffoldContext(view);
    }
}