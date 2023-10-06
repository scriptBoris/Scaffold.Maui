using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit;

public abstract class FlyoutViewBase : ZLayout, IScaffoldProvider, IAppear, IDisappear, IRemovedFromNavigation
{
    private bool isInitialized = true;
    private CancellationTokenSource cancellationTokenSource = new();

    private bool? initialIsPresented;
    private View? initialFlyout;
    private View? initialDetail;

    #region bindable props
    // is presented
    public static readonly BindableProperty IsPresentedProperty = BindableProperty.Create(
        nameof(IsPresented),
        typeof(bool),
        typeof(FlyoutViewBase),
        false,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, o, n) =>
        {
            if (b is not FlyoutViewBase self)
                return;

            if (self.isInitialized)
                self.UpdateFlyoutMenuPresented((bool)n);
            else
                self.initialIsPresented = (bool)n;
        }
    );
    public bool IsPresented
    {
        get => (bool)GetValue(IsPresentedProperty);
        set => SetValue(IsPresentedProperty, value);
    }

    // flyout
    public static readonly BindableProperty FlyoutProperty = BindableProperty.Create(
        nameof(Flyout),
        typeof(View),
        typeof(FlyoutViewBase),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is not FlyoutViewBase self)
                return;

            if (self.isInitialized)
                self.AttachFlyout(n as View);
            else
                self.initialFlyout = n as View;
        }
    );
    public View? Flyout
    {
        get => GetValue(FlyoutProperty) as View;
        set => SetValue(FlyoutProperty, value);
    }

    // detail
    public static readonly BindableProperty DetailProperty = BindableProperty.Create(
        nameof(Detail),
        typeof(View),
        typeof(FlyoutViewBase),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is not FlyoutViewBase self)
                return;

            if (self.isInitialized)
                self.UpdateDetail(n as View, o as View);
            else
                self.initialDetail = n as View;
        }
    );
    public View? Detail
    {
        get => GetValue(DetailProperty) as View;
        set => SetValue(DetailProperty, value);
    }
    #endregion bindable props

    public IScaffold? ProvideScaffold => Detail as IScaffold;

    protected abstract IBackButtonBehavior? BackButtonBehaviorFactory();
    protected abstract void AttachDetail(View detail);
    protected abstract void DeattachDetail(View detail);
    protected abstract void PrepareAnimateSetupDetail(View newDetail, View oldDetail);
    protected abstract Task AnimateSetupDetail(View newDetail, View oldDetail, CancellationToken cancellationToken);
    protected abstract void AttachFlyout(View? flyout);
    protected abstract void UpdateFlyoutMenuPresented(bool isPresented);

    protected void InitializationCompleted()
    {
        isInitialized = true;

        if (initialIsPresented != null)
            UpdateFlyoutMenuPresented(initialIsPresented.Value);

        if (initialFlyout != null)
            AttachFlyout(initialFlyout);

        if (initialDetail != null)
            UpdateDetail(initialDetail, null);
    }

    private async void UpdateDetail(View? view, View? oldDetail)
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
        var cancel = cancellationTokenSource.Token;

        if (view is Scaffold scaffold)
            scaffold.BackButtonBehavior ??= BackButtonBehaviorFactory();

        bool isAnimate = oldDetail != null;
        oldDetail?.TryDisappearing();

        if (view != null)
        {
            // Very important!
            // if detail just view (without binding context) then
            // disable pass current binding context to detail
            if (view.BindingContext == default(object))
                view.BindingContext = null;

            AttachDetail(view);
            view.TryAppearing();

            if (isAnimate)
            {
                await view.AwaitReady(cancel);
                PrepareAnimateSetupDetail(view, oldDetail!);
                var task = AnimateSetupDetail(view, oldDetail!, cancel);
                await task.WithCancelation(cancel);
            }

            view.TryAppearing(true);
        }

        if (oldDetail != null)
        {
            DeattachDetail(oldDetail);
            oldDetail.TryDisappearing(true);
        }
    }

    public void OnAppear(bool isComplete)
    {
        Detail?.TryAppearing(isComplete);
    }

    public void OnDisappear(bool isComplete)
    {
        Detail?.TryDisappearing(isComplete);
    }

    public virtual void OnRemovedFromNavigation()
    {
        if (Detail is IRemovedFromNavigation rm)
            rm.OnRemovedFromNavigation();
    }
}