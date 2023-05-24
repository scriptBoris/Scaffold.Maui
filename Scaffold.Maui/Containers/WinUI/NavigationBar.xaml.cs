using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers.WinUI;

public partial class NavigationBar : INavigationBar
{
    private readonly View _view;

    public NavigationBar(View view)
	{
        _view = view;

		InitializeComponent();

        buttonBack.TapCommand = new Command(() =>
        {
            if (view.GetContext() is Scaffold scaffold)
                scaffold.SoftwareBackButtonInternal();
        });

        UpdateTitle(Scaffold.GetTitle(view));
        UpdateMenuItems(view);
    }

    public async Task UpdateVisual(NavigatingArgs e)
    {
        if (e.NavigationType == NavigatingTypes.Push)
        {
            buttonBack.IsVisible = e.HasBackButton;
            UpdateSafeArea(Scaffold.SafeArea);
            UpdateNavigationBarBackgroundColor(e.NavigationBarBackgroundColor);
            UpdateNavigationBarForegroundColor(e.NavigationBarForegroundColor);
        }

        if (!e.IsAnimating)
            return;

        switch (e.NavigationType)
        {
            case NavigatingTypes.Push:
                this.Opacity = 0;
                //await e.NewContent.AwaitHandler();
                await this.FadeTo(1, Scaffold.AnimationTime);
                break;
            case NavigatingTypes.Pop:
                await this.FadeTo(0, Scaffold.AnimationTime);
                break;
            default:
                break;
        }
    }

    public void UpdateTitle(string? title)
    {
        labelTitle.Text = title;
    }

    public void UpdateBackButtonBehavior(IBackButtonBehavior? behavior)
    {
    }

    public void UpdateMenuItems(View view)
    {
    }

    public void UpdateNavigationBarBackgroundColor(Color color)
    {
        BackgroundColor = color;
    }

    public void UpdateNavigationBarForegroundColor(Color color)
    {
        labelTitle.TextColor = color;
    }

    public void UpdateNavigationBarVisible(bool visible)
    {

    }

    public void UpdateSafeArea(Thickness safeArea)
    {
        HeightRequest = safeArea.Top;
    }
}