using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit.FlyoutViewPlatforms;

namespace ScaffoldLib.Maui.Containers.WinUI;

public partial class NavigationBar : INavigationBar, IWindowsBehavior
{
    private readonly View _view;
    private readonly IScaffold _context;
    private IBackButtonBehavior? backButtonBehavior;

    public NavigationBar(View view)
	{
        _view = view;
        _context = view.GetContext() ?? throw new Exception();

        InitializeComponent();

        buttonBack.TapCommand = new Command(() =>
        {
            if (view.GetContext() is Scaffold scaffold)
                scaffold.SoftwareBackButtonInternal();
        });

        UpdateTitle(Scaffold.GetTitle(view));
        UpdateMenuItems(view);
    }

    public Rect[] UndragArea
    {
        get
        {
            var rects = new List<Rect>();

            if (buttonBack.IsVisible)
                rects.Add(buttonBack.Frame);

            rects.Add(leftViewContainer.Frame);

            return rects.ToArray();
        }
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
        backButtonBehavior = behavior;
        leftViewContainer.Content = behavior?.LeftViewExtended(_context);
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
        imageBackButton.TintColor = color;
    }

    public void UpdateNavigationBarVisible(bool visible)
    {
        IsVisible = visible;
    }

    public void UpdateSafeArea(Thickness safeArea)
    {
        HeightRequest = safeArea.Top;
    }
}