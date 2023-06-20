using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui.Toolkit.FlyoutViewPlatforms;

public partial class FlyoutViewWinUI : FlyoutViewBase
{
    private FlyoutBehavior? currentBehavior;

    public FlyoutViewWinUI()
    {
        InitializeComponent();
        Scaffold.SetHasNavigationBar(this, false);

        buttonMenu.TapCommand = new Command(() =>
        {
            IsPresented = !IsPresented;
        });
        buttonMenu.Content = new ImageTint
        {
            HeightRequest = 18,
            WidthRequest = 18,
            Source = "ic_scaffold_menu.png",
        };
    }

    protected override async Task AnimateSetupDetail(View detail)
    {
        await detail.AwaitHandler();
        await detail.FadeTo(1, 180);
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
                    NavigationBarOffset = 40,
                    ViewOffset = 40,
                };
                scaffold.AddBehavior(currentBehavior);
            }
            Grid.SetColumn(leftPanel, 1);
            //Grid.SetColumn(panelDetail, 0);
            //Grid.SetColumnSpan(panelDetail, 2);
        }
        else
        {
            Grid.SetColumn(leftPanel, 0);
            //Grid.SetColumn(panelDetail, 1);
            //Grid.SetColumnSpan(panelDetail, 1);
        }

        OffsetScaffold(40, 40);
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
        return null;
        //return new FlyoutBackButtonBehavior(this);
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
        //_panelFlyout.IsVisible = true;
        //_panelFlyoutBackground.IsVisible = true;
        //_panelFlyout.TranslateTo(0, 0, 180, Easing.SinIn);
        //_panelFlyoutBackground.FadeTo(1, 180);
        OffsetScaffold(40, 180);
    }

    private async void Hide()
    {
        OffsetScaffold(40, 40);
        //await Task.WhenAll(
        //    _panelFlyout.TranslateTo(-_panelFlyout.Width, 0, 180, Easing.SinOut),
        //    _panelFlyoutBackground.FadeTo(0, 180)
        //);

        //_panelFlyoutBackground.IsVisible = false;
        //_panelFlyout.IsVisible = false;
    }

    private void OffsetScaffold(double navBarOffset, double viewOffset)
    {
        leftPanel.WidthRequest = viewOffset;
        currentBehavior?.Set(navBarOffset, viewOffset);
    }

    public class FlyoutBehavior : IBehavior
    {
        public event SingleDelegate<FlyoutBehavior>? Changed;
        public double NavigationBarOffset { get; set; }
        public double ViewOffset { get; set; }

        public void Set(double navigationBarOffset, double viewOffset)
        {
            NavigationBarOffset = navigationBarOffset;
            ViewOffset = viewOffset;
            Changed?.Invoke(this);
        }
    }
}