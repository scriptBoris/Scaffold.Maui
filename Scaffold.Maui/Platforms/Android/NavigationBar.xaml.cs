using Microsoft.Maui.Controls;
using Scaffold.Maui.Containers;
using Scaffold.Maui.Core;

namespace Scaffold.Maui.Platforms.Android;

public partial class NavigationBar : INavigationBar
{
    private readonly ScaffoldView _carrier;
    private bool currentHasBB;
    private double bbWidth;

    public NavigationBar(ScaffoldView carrier)
	{
        _carrier = carrier;
		InitializeComponent();
		BindingContext = this;

        var m = backButton.Measure(200, 200);
        bbWidth = m.Width + backButton.Margin.HorizontalThickness + backButton.Padding.HorizontalThickness;
        //backButton.IsVisible = false;

        gridTitles.TranslationX = -bbWidth;
        backButton.Scale = 0;
        backButton.TapCommand = new Command(() => carrier.SoftwareBackButtonInternal());
    }

    private DataTemplate TitleTemplate => (DataTemplate)this.Resources["titleTemplate"];
    private DataTemplate MenuTemplate => (DataTemplate)this.Resources["menuTemplate"];

    public async Task SwitchContent(NavigationSwitchArgs args)
    {
        bool isEmpty = gridTitles.Children.Count == 0;
        bool isAnimating = args.IsAnimating && !isEmpty;
        bool hasNavigationBar = ScaffoldView.GetHasNavigationBar(args.NewContent);

        UpdateNavigationBarVisible(hasNavigationBar, isAnimating);

        if (hasNavigationBar)
        {
            UpdateMenu(args.NewContent, isAnimating);
            UpdateBackbutton(args.HasBackButton, isAnimating);
            UpdateTitle(args.NewContent, isAnimating);
        }

        await Task.Delay(ScaffoldView.AnimationTime);
    }

    public void Dispose()
    {
    }

    private void UpdateNavigationBarVisible(bool isVisible, bool isAnimating)
    {
        if (isAnimating)
        {
            if (isVisible)
            {
                var anim = new Animation((x) =>
                {
                    TranslationY = x;
                }, TranslationY, 0);
                anim.Commit(this, "trans", length: ScaffoldView.AnimationTime);
            }
            else
            {
                var anim = new Animation((x) =>
                {
                    TranslationY = x;
                }, TranslationY, -this.Height);
                anim.Commit(this, "trans", length: ScaffoldView.AnimationTime);
            }
        }
        else
        {
            this.IsVisible = isVisible;
        }
    }

    private async void UpdateBackbutton(bool isVisible, bool isAnimating)
    {
        if (currentHasBB == isVisible)
            return;

        currentHasBB = isVisible;

        if (isAnimating)
        {
            if (isVisible)
            {
                gridTitles.TranslationX = -bbWidth;
                await Task.WhenAll(
                    backButton.ScaleTo(1, ScaffoldView.AnimationTime),
                    gridTitles.TranslateTo(0, 0, ScaffoldView.AnimationTime));
            }
            else
            {
                var to = -bbWidth;
                await Task.WhenAll(
                    backButton.ScaleTo(0, ScaffoldView.AnimationTime),
                    gridTitles.TranslateTo(to, 0, ScaffoldView.AnimationTime));
            }
        }
        else
        {
            backButton.Scale = isVisible ? 1 : 0;
            gridTitles.TranslationX = isVisible ? 0 : -bbWidth;
        }
    }

    private async void UpdateTitle(View view, bool isAnimating)
    {
        var title = (Label)TitleTemplate.CreateContent();
        title.Text = ScaffoldView.GetTitle(view);

        foreach (View item in gridTitles.Children)
            ClearTitle(item, isAnimating);

        gridTitles.Children.Add(title);

        if (isAnimating)
        {
            title.Opacity = 0;
            await title.FadeTo(1, ScaffoldView.AnimationTime);
        }
    }

    private async void UpdateMenu(View view, bool isAnimating)
    {
        var menus = ScaffoldView.GetMenuItems(view);

        foreach(View menu in gridMenu.Children)
            ClearMenu(menu, isAnimating);

        var stackMenus = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Margin = new Thickness(0,0,7,0),
            Opacity = menus.Count > 0 ? 0 : 1,
        };

        foreach (var item in menus)
        {
            var b = (View)MenuTemplate.CreateContent();
            b.BindingContext = item;
            stackMenus.Children.Add(b);
        }

        gridMenu.Children.Add(stackMenus);

        if (menus.Count > 0)
            await stackMenus.FadeTo(1, ScaffoldView.AnimationTime);
    }

    private async void ClearMenu(View view, bool isAnimating)
    {
        if (isAnimating)
            await view.FadeTo(0, ScaffoldView.AnimationTime);
        gridMenu.Children.Remove(view);
    }

    private async void ClearTitle(View title, bool isAnimating)
    {
        if (isAnimating)
            await title.FadeTo(0, ScaffoldView.AnimationTime);
        gridTitles.Children.Remove(title);
    }
}