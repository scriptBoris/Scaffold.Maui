using Microsoft.Maui.Controls;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui.Toolkit.FlyoutViewPlatforms;

public partial class FlyoutViewMaterial : FlyoutViewBase
{
	public FlyoutViewMaterial() : base()
	{
		InitializeComponent();
        Scaffold.SetHasNavigationBar(this, false);

        gestureTap.Command = new Command(() =>
        {
            IsPresented = false;
        });
    }

    private bool isFirst = true;
    public override Size Measure(double widthConstraint, double heightConstraint)
    {
        var w = widthConstraint * 0.7;
        _panelFlyout.WidthRequest = w;

        //if (!IsPresented)
        if (isFirst && !IsPresented)
        {
            _panelFlyout.TranslationX = -w;
            isFirst = false;
        }

        return base.Measure(widthConstraint, heightConstraint);
    }

    protected override async Task AnimateSetupDetail(View detail)
    {
        await detail.AwaitHandler();
        await detail.FadeTo(1, 180);
    }

    protected override void AttachDetail(View detail)
    {
        panelDetail.Children.Add(detail);
    }

    protected override void DeattachDetail(View detail)
    {
        panelDetail.Children.Remove(detail);
    }

    protected override void AttachFlyout(View? flyout)
    {
        _panelFlyout.Content = flyout;
    }

    protected override IBackButtonBehavior? BackButtonBehaviorFactory()
    {
        return new FlyoutBackButtonBehavior(this);
    }

    protected override void UpdateFlyoutMenuPresented(bool isPresented)
    {
        if (isPresented)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        _panelFlyout.IsVisible = true;
        _panelFlyoutBackground.IsVisible = true;
        _panelFlyout.TranslateTo(0, 0, 180, Easing.SinIn);
        _panelFlyoutBackground.FadeTo(1, 180);
    }

    private async void Hide()
    {
        await Task.WhenAll(
            _panelFlyout.TranslateTo(-_panelFlyout.Width, 0, 180, Easing.SinOut),
            _panelFlyoutBackground.FadeTo(0, 180)
        );

        _panelFlyoutBackground.IsVisible = false;
        _panelFlyout.IsVisible = false;
    }

    public class FlyoutBackButtonBehavior : IBackButtonBehavior
    {
        private readonly FlyoutViewMaterial _parent;

        public FlyoutBackButtonBehavior(FlyoutViewMaterial parent)
        {
            _parent = parent;
        }

        public ImageSource? OverrideBackButtonIcon(IScaffold context)
        {
            if (context.NavigationStack.Count <= 1)
                return ImageSource.FromFile("ic_scaffold_menu.png");

            return null;
        }

        public bool? OverrideBackButtonVisibility(IScaffold context)
        {
            return true;
        }

        public bool? OverrideSoftwareBackButtonAction(IScaffold context)
        {
            if (context.NavigationStack.Count <= 1)
            {
                _parent.IsPresented = !_parent.IsPresented;
                return true;
            }

            return null;
        }

        public bool? OverrideHardwareBackButtonAction(IScaffold context)
        {
            if (context.NavigationStack.Count <= 1 && _parent.IsPresented)
            {
                _parent.IsPresented = false;
                return true;
            }

            return null;
        }
    }

}