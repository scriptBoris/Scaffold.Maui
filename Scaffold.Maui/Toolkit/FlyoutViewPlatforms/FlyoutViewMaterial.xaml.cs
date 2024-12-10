using ButtonSam.Maui.Core;
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

    protected override void PrepareAnimateSetupDetail(View newDetail, View oldDetail)
    {
        newDetail.Opacity = 0;
    }

    protected override Task AnimateSetupDetail(View detail, View oldDetail, CancellationToken cancellationToken)
    {
        return detail.FadeTo(1, 180);
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
        this.BatchBegin();
        _panelFlyout.IsVisible = true;
        _panelFlyoutBackground.IsVisible = true;
        this.BatchCommit();

        this.TransitAnimation("show", 0, 1, 180, Easing.SinIn, (x) =>
        {
            this.BatchBegin();
            _panelFlyout.TranslationX *= 1 - x;
            _panelFlyoutBackground.Opacity = x;
            this.BatchCommit();
        });
    }

    private async void Hide()
    {
        bool success = await this.TransitAnimation("hide", 1, 0, 180, Easing.SinOut, (x) =>
        {
            this.BatchBegin();
            double mod = 1 - x;
            _panelFlyout.TranslationX = -(_panelFlyout.Width * mod);
            _panelFlyoutBackground.Opacity = x;
            this.BatchCommit();
        });

        if (success)
        {
            this.BatchBegin();
            _panelFlyoutBackground.IsVisible = false;
            _panelFlyout.IsVisible = false;
            this.BatchCommit();
        }
    }

    public class FlyoutBackButtonBehavior : IBackButtonBehavior
    {
        private readonly FlyoutViewMaterial _parent;

        public FlyoutBackButtonBehavior(FlyoutViewMaterial parent)
        {
            _parent = parent;
        }

        public ImageSource? OverrideBackButtonIcon(IAgent agent, IScaffold context)
        {
            if (agent.IndexInNavigationStack == 0)
                return ImageSource.FromFile("scaffoldlib_flyout_menu.png");

            return null;
        }

        public bool? OverrideBackButtonVisibility(IAgent agent, IScaffold context)
        {
            return true;
        }

        public bool? OverrideSoftwareBackButtonAction(IAgent agent, IScaffold context)
        {
            if (agent.IndexInNavigationStack == 0)
            {
                _parent.IsPresented = !_parent.IsPresented;
                return true;
            }

            return null;
        }

        public bool? OverrideHardwareBackButtonAction(IAgent agent, IScaffold context)
        {
            if (agent.IndexInNavigationStack == 0 && _parent.IsPresented)
            {
                _parent.IsPresented = false;
                return true;
            }

            return null;
        }
    }
}