using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit.FlyoutViewPlatforms;

public abstract class FlyoutViewBase : ZLayout, IScaffoldProvider, IAppear, IDisappear, INavigationMember
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

        BatchBegin();

        if (initialIsPresented != null)
            UpdateFlyoutMenuPresented(initialIsPresented.Value);

        if (initialFlyout != null)
            AttachFlyout(initialFlyout);

        if (initialDetail != null)
            UpdateDetail(initialDetail, null);

        BatchCommit();
    }

    private async void UpdateDetail(View? newDetail, View? oldDetail)
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
        var cancel = cancellationTokenSource.Token;

        if (newDetail is Scaffold scaffold)
            scaffold.BackButtonBehavior ??= BackButtonBehaviorFactory();

        // Start
        BatchBegin();

        bool isAnimate = oldDetail != null;
        oldDetail?.TryDisappearing();

        if (newDetail != null)
        {
            // Very important!
            // if detail just view (without binding context) then
            // disable pass current binding context to detail
            if (newDetail.BindingContext == default)
                newDetail.BindingContext = null;

            AttachDetail(newDetail);
            newDetail.TryAppearing();

            if (isAnimate)
            {
                PrepareAnimateSetupDetail(newDetail, oldDetail!);
                
                BatchCommit();

                await newDetail.AwaitReady(cancel);
                var task = AnimateSetupDetail(newDetail, oldDetail!, cancel);
                await task.WithCancelation(cancel);
            }

            newDetail.TryAppearing(true);
        }

        if (oldDetail != null)
        {
            DeattachDetail(oldDetail);
            oldDetail.TryDisappearing(true);
        }

        if (!isAnimate)
            BatchCommit();
    }

    public void OnAppear(bool isComplete)
    {
        Detail?.TryAppearing(isComplete);
    }

    public void OnDisappear(bool isComplete)
    {
        Detail?.TryDisappearing(isComplete);
    }

    public virtual void OnConnectedToNavigation()
    {
        if (Detail is INavigationMember member)
            member.OnConnectedToNavigation();
    }

    public virtual void OnDisconnectedFromNavigation()
    {
        if (Detail is INavigationMember rm)
            rm.OnDisconnectedFromNavigation();
    }
}