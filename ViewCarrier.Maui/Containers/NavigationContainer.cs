using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scaffold.Maui.Core;
using Scaffold.Maui.Internal;

namespace Scaffold.Maui.Containers;

public class NavigationContainer : Layout, ILayoutManager, IDisposable
{
    private readonly ObservableCollection<View> _navigationStack = new();
    private readonly ObservableCollection<NavigationItem> _logicalChildren = new();

    public event EventHandler<PushEventArgs>? PushStarted;
    public event EventHandler<PushEventArgs>? PushFinished;
    public event EventHandler<PopEventStartedArgs>? PopStarted;
    public event EventHandler<PopEventArgs>? PopFinished;

    public NavigationContainer()
    {
        NavigationStack = new(_navigationStack);
        LogicalChildren = new(_logicalChildren);
    }

    public ReadOnlyObservableCollection<View> NavigationStack { get; init; }
    public new ReadOnlyObservableCollection<NavigationItem> LogicalChildren { get; init; }
    private NavigationItem? Current => LogicalChildren.LastOrDefault();

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    public Size ArrangeChildren(Rect bounds)
    {
        foreach (IView child in Children)
            child.Arrange(bounds);

        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        foreach (IView child in Children)
            child.Measure(widthConstraint, heightConstraint);

        return new Size(widthConstraint, heightConstraint);
    }

    internal async Task PushAsync(View view, bool isAnimated)
    {
        var current = Current;

        if (isAnimated && current == null)
            isAnimated = false;

        PushStarted?.Invoke(this, new PushEventArgs
        {
            IsAnimated = isAnimated,
            HasBackButton = current != null,
            PushView = view,
            PreviousView = current?.View,
        });

        var navItem = new NavigationItem(view);

        Children.Add(navItem);
        _navigationStack.Add(view);
        _logicalChildren.Add(navItem);

        if (isAnimated)
        {
            navItem.Opacity = 0;
            navItem.TranslationX = 100;
            await Task.WhenAll(
                //current?.FadeTo(0.5, ScaffoldView.AnimationPushTime) ?? Task.CompletedTask,
                navItem.FadeTo(1, ScaffoldView.AnimationTime),
                navItem.TranslateTo(0, 0, ScaffoldView.AnimationTime, Easing.CubicOut)
            );
        }

        if (current != null)
            current.IsVisible = false;

        PushFinished?.Invoke(this, new PushEventArgs
        {
            IsAnimated = isAnimated,
            HasBackButton = current != null,
            PushView = view,
            PreviousView = current?.View,
        });
    }

    internal Task InserAsync(View view, int index, bool isAnimated)
    {
        throw new NotImplementedException();
    }

    internal async Task PopAsync(bool isAnimated)
    {
        // todo hide keyboard
//#if ANDROID
//        Platforms.Android.Callbacks.HideKeyboard.Hide();
//#elif IOS
//        Platforms.iOS.Callbacks.HideKeyboard.Hide();
//#endif
        var popedItem = Current;
        if (popedItem == null || _logicalChildren.Count == 1)
            return;

        var upderPopedItem = _logicalChildren.ItemOrDefault(_logicalChildren.Count - 2);
        bool hasBackButton = _logicalChildren.ItemOrDefault(_logicalChildren.Count - 3) != null;

        if (upderPopedItem != null)
        {
            upderPopedItem.Opacity = 1;
            upderPopedItem.IsVisible = true;
        }

        PopStarted?.Invoke(this, new PopEventStartedArgs
        {
            IsAnimated = isAnimated,
            IsBackButtonDissapear = !hasBackButton,
            PopedView = popedItem.View,
            UnderPopedView = upderPopedItem?.View,
        });

        if (isAnimated)
        {
            await Task.WhenAll(
                popedItem.FadeTo(0, ScaffoldView.AnimationTime, Easing.CubicOut),
                popedItem.TranslateTo(50, 0, ScaffoldView.AnimationTime, Easing.CubicOut)
            );
        }

        Children.Remove(popedItem);
        _logicalChildren.Remove(popedItem);
        _navigationStack.Remove(popedItem.View);

        PopFinished?.Invoke(this, new PopEventArgs
        {
            IsAnimated = isAnimated,
            HasBackButton = hasBackButton,
            PopedView = popedItem.View,
            UnderPopedView = upderPopedItem?.View,
        });

        popedItem.Dispose();
    }

    public async Task PopToRoot(bool isAnimated)
    {
        if (_logicalChildren.Count <= 1)
            return;

        if (_logicalChildren.Count == 2)
            await PopAsync(isAnimated);

        for (int i = _logicalChildren.Count - 2; i > 0; i--)
        {
            var item = _logicalChildren[i];
            item.Dispose();
            _logicalChildren.Remove(item);
            _navigationStack.Remove(item.View);
        }

        await PopAsync(isAnimated);
    }

    public void Dispose()
    {
        foreach (var item in LogicalChildren)
            item.Dispose();
    }
}

public class PushEventArgs
{
    public required bool IsAnimated { get; set; }
    public required bool HasBackButton { get; set; }
    public required View PushView { get; set; }
    public View? PreviousView { get; set; }
}

public class PopEventStartedArgs
{
    public required bool IsAnimated { get; set; }
    public required bool IsBackButtonDissapear { get; set; }
    public required View PopedView { get; set; }
    public View? UnderPopedView { get; set; }
}

public class PopEventArgs
{
    public required bool IsAnimated { get; set; }
    public required bool HasBackButton { get; set; }
    public required View PopedView { get; set; }
    public View? UnderPopedView { get; set; }
}