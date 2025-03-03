using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui.Toolkit.FlyoutViewPlatforms;

public partial class FlyoutViewWinUI : FlyoutViewBase, IWindowsBehavior
{
    private FlyoutBehavior? currentBehavior;
    private const int Min = 40;
    private const int Max = 200;
    private readonly StaticLibs.ButtonSam.Button _flyoutButton;
    private readonly ImageTint _flyoutImage;
    private double _flyoutPanelShowPercent;

    public FlyoutViewWinUI()
    {
        InitializeComponent();
        Scaffold.SetHasNavigationBar(this, false);

        _flyoutButton = new StaticLibs.ButtonSam.Button
        {
            Margin = 5,
            Padding = 6,
            BackgroundColor = Colors.Transparent,
            CornerRadius = 5,
            HorizontalOptions = LayoutOptions.Start,
            Content = _flyoutImage = new ImageTint
            {
                HeightRequest = 18,
                WidthRequest = 18,
                Source = "scaffoldlib_flyout_menu.png",
            },
            TapCommand = new Command(() =>
            {
                IsPresented = !IsPresented;
            }),
        };
        _flyoutImage.SetAppTheme(ImageTint.TintColorProperty, Colors.Gray, Colors.White);
        panelFlyout.Children.Insert(0, _flyoutButton);

        if (IsPresented)
            _flyoutPanelShowPercent = 1;
    }

    public Rect[] UndragArea => new Rect[]
    {
        new Rect(0,0, _flyoutButton.Width, _flyoutButton.Height),
    };

    protected override void PrepareAnimateSetupDetail(View newDetail, View oldDetail)
    {
        newDetail.Opacity = 0;
    }

    protected override async Task AnimateSetupDetail(View detail, View oldDetail, CancellationToken cancel)
    {
        View? resetView = null;
        if (oldDetail is IScaffold oldScaffold && oldScaffold is View vold)
        {
            var actual = oldScaffold.NavigationStack.LastOrDefault();
            if (actual == null)
                goto defaultAnim;

            var agent = Scaffold.FindAgent(actual)?.ViewWrapper as View;
            if (agent == null)
                goto defaultAnim;

            agent.Opacity = 0;
            resetView = agent;
        }

        if (detail is IScaffold sc)
        {
            var actual = sc.NavigationStack.LastOrDefault();
            if (actual == null)
                goto defaultAnim;

            var agent = Scaffold.FindAgent(actual)?.ViewWrapper as View;
            if (agent == null)
                goto defaultAnim;

            agent.TranslationY = 100;
            await Task.WhenAll(
                 detail.FadeTo(1, 180),
                 agent.TranslateTo(0, 0, 180, Easing.SinInOut)
            );
        }

    defaultAnim:
        await detail.FadeTo(1, 180);

        if (resetView != null)
        {
            resetView.Opacity = 1;
        }
    }

    protected override void AttachDetail(View detail)
    {
        if (detail is Scaffold scaffold)
        {
            currentBehavior = scaffold.ExternalBevahiors.FirstOrDefault(x => x is FlyoutBehavior) as FlyoutBehavior;
            if (currentBehavior == null)
            {
                currentBehavior = new FlyoutBehavior
                {
                    NavigationBarOffset = Min,
                    ViewOffset = Min,
                    OnChanged = () =>
                    {
                        IsPresented = !IsPresented;
                    }
                };
                scaffold.AddBehavior(currentBehavior);
            }
            //Grid.SetColumn(leftPanel, 1);
            //Grid.SetColumn(panelDetail, 0);
            //Grid.SetColumnSpan(panelDetail, 2);
        }
        else
        {
            //Grid.SetColumn(leftPanel, 0);
            //Grid.SetColumn(panelDetail, 1);
            //Grid.SetColumnSpan(panelDetail, 1);
        }

        if (IsPresented)
            OffsetScaffold(Min, 1);
        else
            OffsetScaffold(Min, 0);

        panelDetail.Children.Add(detail);
    }

    protected override void DeattachDetail(View detail)
    {
        panelDetail.Children.Remove(detail);
    }

    protected override void AttachFlyout(View? flyout)
    {
        panelFlyout_customContent.Content = flyout;
    }

    protected override IBackButtonBehavior? BackButtonBehaviorFactory()
    {
        return null;
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
        this.Animate("flyout", (x) =>
        {
            OffsetScaffold(Min, x);
        },
        start: _flyoutPanelShowPercent,
        end: 1,
        length: 200);
    }

    private void Hide()
    {
        this.Animate("flyout", (x) =>
        {
            OffsetScaffold(Min, x);
        },
        start: _flyoutPanelShowPercent,
        end: 0,
        length: 200);
    }

    private void OffsetScaffold(double navBarOffset, double showPercent)
    {
        _flyoutPanelShowPercent = showPercent;
        double viewOffset = double.Lerp(Min, Max, showPercent);
        panelFlyout.WidthRequest = viewOffset;
        panelFlyout_blocker.Opacity = showPercent;

        if (showPercent > 0)
            panelFlyout_blocker.IsVisible = true;
        else
            panelFlyout_blocker.IsVisible = false;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        IsPresented = false;
    }

    public class FlyoutBehavior : IBehavior
    {
        public FlyoutBehavior()
        {
        }

        public event SingleDelegate<FlyoutBehavior>? Changed;
        public double NavigationBarOffset { get; set; }
        public double ViewOffset { get; set; }
        public required Action OnChanged { private get; set; }

        public void Set(double navigationBarOffset, double viewOffset)
        {
            NavigationBarOffset = navigationBarOffset;
            ViewOffset = viewOffset;
            Changed?.Invoke(this);
        }

        public void InvokeChangePresented()
        {
            OnChanged?.Invoke();
        }
    }
}