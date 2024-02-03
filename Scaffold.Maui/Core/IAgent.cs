using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Layouts;
using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ScaffoldLib.Maui.Core;

public interface IAgent : IDisposable
{
    IScaffold Context { get; }
    IViewWrapper ViewWrapper { get; }
    INavigationBar? NavigationBar { get; }
    IZBuffer ZBuffer { get; }

    bool IsAppear { get; internal set; }
    int IndexInNavigationStack { get; internal set; }
    Color NavigationBarBackgroundColor { get; internal set; }
    Color NavigationBarForegroundColor { get; internal set; }
    bool HasNavigationBar { get; internal set; }
    IBackButtonBehavior? BackButtonBehavior { get; internal set; }
    Thickness SafeArea { get; internal set; }

    void PrepareAnimate(NavigatingTypes navigationType);
    AnimationInfo GetAnimation(NavigatingTypes animationType);
    void DoAnimation(double x, NavigatingTypes animType);
    Task DoPlatformAnimation(NavigatingTypes animType);
    void RestoreVisualState();

    void OnBackButton();
    void OnMenuButton();

    void OnBehaiorAdded(IBehavior newBehaior);
    void OnBehaiorRemoved(IBehavior removedBehaior);
}

[DebuggerDisplay($"Agent :: {{{nameof(ViewType)}}}")]
public abstract class Agent : Layout, IAgent, ILayoutManager, IDisposable, IAppear, IDisappear, IRemovedFromNavigation
{
    private readonly View _view;
    private INavigationBar? _navigationBar;
    private Thickness _safeArea;
    private Color _navigationBarBackgroundColor;
    private Color _navigationBarForegroundColor;
    private int _indexInNavigationStack;
    private IBackButtonBehavior? _backButtonBehavior;
    private bool isBackButtonPressed;

    public Agent(CreateAgentArgs args)
    {
        _view = args.View;
        Context = args.Context;
        ViewWrapper = ((Scaffold)Context).ViewFactory.CreateViewWrapper(new Args.CreateViewWrapperArgs
        {
            View = _view,
            Context = Context,
        });
        Children.Add((View)ViewWrapper);

        _navigationBarBackgroundColor = args.NavigationBarBackgroundColor;
        _navigationBarForegroundColor = args.NavigationBarForegroundColor;
        _indexInNavigationStack = args.IndexInStack;
        _backButtonBehavior = args.BackButtonBehavior;
        UpdateNavigationBar();

        ZBuffer = new ZBuffer();
        Children.Add((View)ZBuffer);

        SafeArea = args.SafeArea;
    }

    public IScaffold Context { get; private set; }
    public bool IsAppear { get; set; }
    public IViewWrapper ViewWrapper { get; private set; }
    public INavigationBar? NavigationBar
    {
        get => _navigationBar;
        private set
        {
            if (_navigationBar != null)
            {
                Children.Remove((IView)_navigationBar);
            }

            _navigationBar = value;

            if (_navigationBar != null)
            {
                if (Children.Count == 0)
                    Children.Add((IView)_navigationBar);
                else
                    Children.Insert(1, (IView)_navigationBar);
            }
        }
    }
    public IZBuffer ZBuffer { get; private set; }

    public virtual int IndexInNavigationStack
    {
        get => _indexInNavigationStack;
        set
        {
            _indexInNavigationStack = value;
            NavigationBar?.UpdateBackButtonVisibility(value > 0);
        }
    }
    public virtual Thickness SafeArea
    {
        get => _safeArea;
        set
        {
            _safeArea = value;
            NavigationBar?.UpdateSafeArea(value);
            ViewWrapper.UpdateSafeArea(value);
        }
    }
    public bool HasNavigationBar
    {
        get => _navigationBar != null;
        set
        {
            UpdateNavigationBar();
        }
    }
    public virtual Color NavigationBarBackgroundColor
    {
        get => _navigationBarBackgroundColor;
        set
        {
            _navigationBarBackgroundColor = value;
            NavigationBar?.UpdateNavigationBarBackgroundColor(value);
        }
    }
    public virtual Color NavigationBarForegroundColor
    {
        get => _navigationBarForegroundColor;
        set
        {
            _navigationBarForegroundColor = value;
            NavigationBar?.UpdateNavigationBarForegroundColor(value);
        }
    }
    public IBackButtonBehavior? BackButtonBehavior 
    {
        get => _backButtonBehavior;
        set
        {
            _backButtonBehavior = value;
            NavigationBar?.UpdateBackButtonBehavior(value);
        }
    }

    internal string ViewType => _view.GetType().Name;

    public virtual Size ArrangeChildren(Rect bounds)
    {
        double offsetY = 0;

        if (NavigationBar is IView bar)
        {
            offsetY = bar.DesiredSize.Height;
            bar.Arrange(new Rect(0, 0, bounds.Width, bar.DesiredSize.Height));
        }

        bool isUnderNavBar = Scaffold.GetIsContentUnderNavigationBar(_view);
        if (isUnderNavBar)
            offsetY = 0;

        if (ViewWrapper is IView view)
        {
            //double h = bounds.Height - offsetY;
            double h = view.DesiredSize.Height;
            view.Arrange(new Rect(0, offsetY, bounds.Width, h));
        }

        if (ZBuffer is IView zbuffer)
        {
            zbuffer.Arrange(bounds);
        }

        return bounds.Size;
    }

    public virtual Size Measure(double widthConstraint, double heightConstraint)
    {
        double freeH = heightConstraint;

        if (NavigationBar is IView bar)
        {
            var m = bar.Measure(widthConstraint, freeH);
            freeH -= m.Height;
        }

        bool isUnderNavBar = Scaffold.GetIsContentUnderNavigationBar(_view);
        if (isUnderNavBar)
            freeH = heightConstraint;

        if (ViewWrapper is IView view)
        {
            view.Measure(widthConstraint, freeH);
        }

        if (ZBuffer is IView zbuffer)
        {
            zbuffer.Measure(widthConstraint, heightConstraint);
        }

        return new Size(widthConstraint, heightConstraint);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    private void UpdateNavigationBar()
    {
        bool oldIsVisible = NavigationBar != null;
        bool isVisible = ScaffoldLib.Maui.Scaffold.GetHasNavigationBar(_view);
        if (oldIsVisible != isVisible)
        {
            if (isVisible)
            {
                var navBar = Context.ViewFactory.CreateNavigationBar(new Args.CreateNavigationBarArgs
                {
                    Agent = this,
                    View  = _view,
                });
                if (navBar != null)
                {
                    navBar.UpdateSafeArea(SafeArea);
                    navBar.UpdateNavigationBarForegroundColor(NavigationBarForegroundColor);
                    navBar.UpdateNavigationBarBackgroundColor(NavigationBarBackgroundColor);
                    navBar.UpdateBackButtonVisibility(IndexInNavigationStack > 0);
                    navBar.UpdateTitle(Scaffold.GetTitle(_view));
                    navBar.UpdateMenuItems(Scaffold.GetMenuItems(_view));
                    navBar.UpdateBackButtonBehavior(BackButtonBehavior);
                }
                NavigationBar = navBar;
            }
            else
            {
                NavigationBar = null;
            }
        }
    }

    public virtual void OnBehaiorAdded(IBehavior newBehaior) { }
    public virtual void OnBehaiorRemoved(IBehavior removedBehaior) { }
    public virtual void PrepareAnimate(NavigatingTypes type) { }
    public virtual AnimationInfo GetAnimation(NavigatingTypes underPush) => new AnimationInfo();
    public virtual void DoAnimation(double x, NavigatingTypes animType) { }
    public virtual Task DoPlatformAnimation(NavigatingTypes animType) => Task.CompletedTask;
    public virtual void RestoreVisualState()
    {
        TranslationX = 0;
        TranslationY = 0;
        Scale = 1;
        Opacity = 1;
    }

    public virtual async void OnBackButton()
    {
        if (isBackButtonPressed)
            return;

        isBackButtonPressed = true;

        if (BackButtonBehavior?.OverrideSoftwareBackButtonAction(this, Context) == true)
        {
            isBackButtonPressed = false;
            return;
        }
        
        await this.Dispatcher.DispatchAsync(async () =>
        {
            IBackButtonListener ls;
            if (ViewWrapper.View is IBackButtonListener v)
                ls = v;
            else if (ViewWrapper.View.BindingContext is IBackButtonListener vm)
                ls = vm;
            else
                ls = (Scaffold)Context;

            bool canPop = await ls.OnBackButton();
            if (canPop)
                _ = Context.PopAsync().ConfigureAwait(false);
        });
        isBackButtonPressed = false;
    }

    public virtual void OnMenuButton()
    {
        var overlay = Context.ViewFactory.CreateCollapsedMenuItemsLayer(new Args.CreateCollapsedMenuArgs
        {
            Context = Context,
            View = _view,
        });
        ZBuffer.AddLayer(overlay, IScaffold.MenuItemsIndexZ);
    }

    public virtual void Dispose()
    {
        foreach (var item in Children)
        {
            if (item is IDisposable disposable)
                disposable.Dispose();
        }
        Handler = null;
    }

    public virtual void OnAppear(bool isComplete)
    {
    }

    public virtual void OnDisappear(bool isComplete)
    {
    }

    public virtual void OnRemovedFromNavigation()
    {
    }
}
