using Android.Content;
using Microsoft.Maui.Controls;
using Scaffold.Maui.Containers;
using Scaffold.Maui.Core;

namespace Scaffold.Maui.Platforms.Android;

public partial class NavigationBar : INavigationBar
{
    private readonly View _view;
    private readonly IScaffold _context;
    private MenuItemObs? itemObs;
    private IBackButtonBehavior? backButtonBehavior;
    private const double materialNavBarHeight = 56;

    public NavigationBar(View view)
	{
        _view = view;
        _context = view.GetContext() ?? throw new Exception();
        InitializeComponent();
        backButton.TapCommand = new Command(OnBackButton);
        buttonMenu.TapCommand = new Command(OnMenuButton);

        UpdateTitle(view);
        UpdateMenuItems(view);
    }

    public string? Title 
    { 
        get => labelTitle.Text;
        set
        {
            labelTitle.Text = value;
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (this.Handler?.PlatformView is global::Android.Views.View aview)
            aview.Elevation = 4;
    }

    private void CollapsedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateMenuItems(_view);
    }

    private void OnBackButton()
    {
        if (backButtonBehavior?.OverrideSoftwareBackButtonAction(_context) == true)
            return;

        if (_context is ScaffoldView scaffold)
            scaffold.SoftwareBackButtonInternal();
    }

    private void OnMenuButton()
    {
        if (_view.GetContext() is ScaffoldView scaffold)
            scaffold.ShowCollapsedMenusInternal(_view);
    }

    public async Task UpdateVisual(NavigatingArgs e)
    {
        Padding = e.SafeArea;
        UpdateBackButtonVisual(e.HasBackButton);

        if (!e.IsAnimating)
            return;

        switch (e.NavigationType)
        {
            case NavigatingTypes.Push:
                this.Opacity = 0;
                await this.FadeTo(1, ScaffoldView.AnimationTime);
                break;
            case NavigatingTypes.Pop:
                await this.FadeTo(0, ScaffoldView.AnimationTime);
                break;
            default:
                break;
        }
    }

    public void Dispose()
    {
        if (itemObs != null)
            itemObs.CollapsedItems.CollectionChanged -= CollapsedItems_CollectionChanged;
    }

    private void UpdateTitle(View view)
    {
        labelTitle.Text = ScaffoldView.GetTitle(view);
    }

    public void UpdateMenuItems(View view)
    {
        if (itemObs != null)
            itemObs.CollapsedItems.CollectionChanged -= CollapsedItems_CollectionChanged;

        itemObs = ScaffoldView.GetMenuItems(view);
        itemObs.CollapsedItems.CollectionChanged += CollapsedItems_CollectionChanged;
        bool colapseVisible = itemObs.CollapsedItems.Count > 0;

        BindableLayout.SetItemsSource(stackMenu, itemObs.VisibleItems);
        buttonMenu.IsVisible = colapseVisible;
    }

    public void UpdateNavigationBarVisible(bool visible)
    {
        IsVisible = visible;
    }

    private void UpdateBackButtonVisual(bool defaultVisible)
    {
        var src = backButtonBehavior?.OverrideBackButtonIcon(_context);
        imageBackButton.Source = src ?? "ic_arrow_left.png";

        var visible = backButtonBehavior?.OverrideBackButtonVisibility(_context);
        backButton.IsVisible = visible ?? defaultVisible;
    }

    public void UpdateBackButtonBehavior(IBackButtonBehavior? behavior)
    {
        backButtonBehavior = behavior;
        UpdateBackButtonVisual(backButton.IsVisible);
    }
}