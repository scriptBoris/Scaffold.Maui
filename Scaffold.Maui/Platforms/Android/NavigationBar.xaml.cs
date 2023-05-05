using Microsoft.Maui.Controls;
using Scaffold.Maui.Containers;
using Scaffold.Maui.Core;

namespace Scaffold.Maui.Platforms.Android;

public partial class NavigationBar : INavigationBar
{
    private readonly View _view;
    private MenuItemObs? itemObs;

    public NavigationBar(View view)
	{
        _view = view;
		InitializeComponent();
        backButton.TapCommand = new Command(OnBackButton);
        buttonMenu.TapCommand = new Command(OnMenuButton);


        UpdateTitle(view);
        UpdateMenu(view);
    }

    public string? Title 
    { 
        get => labelTitle.Text;
        set
        {
            labelTitle.Text = value;
        }
    }

    private void CollapsedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateMenu(_view);
    }

    private void OnBackButton()
    {
        if (_view.GetContext() is ScaffoldView scaffold)
            scaffold.SoftwareBackButtonInternal();
    }

    private void OnMenuButton()
    {
        if (_view.GetContext() is ScaffoldView scaffold)
            scaffold.ShowCollapsedMenusInternal(_view);
    }

    public async Task UpdateVisual(NavigatingArgs e)
    {
        backButton.IsVisible = e.HasBackButton;

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

    public void UpdateMenu(View view)
    {
        if (itemObs != null)
            itemObs.CollapsedItems.CollectionChanged -= CollapsedItems_CollectionChanged;

        itemObs = ScaffoldView.GetMenuItems(view);
        itemObs.CollapsedItems.CollectionChanged += CollapsedItems_CollectionChanged;
        bool colapseVisible = itemObs.CollapsedItems.Count > 0;

        BindableLayout.SetItemsSource(stackMenu, itemObs.VisibleItems);
        buttonMenu.IsVisible = colapseVisible;
    }
}