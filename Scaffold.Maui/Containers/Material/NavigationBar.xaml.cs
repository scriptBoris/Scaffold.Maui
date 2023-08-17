using Microsoft.Maui.Controls;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using MenuItemCollection = ScaffoldLib.Maui.Core.MenuItemCollection;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class NavigationBar : INavigationBar, IDisposable
{
    private readonly View _view;
    private readonly IScaffold _context;
    private MenuItemCollection? menuItems;
    private IBackButtonBehavior? backButtonBehavior;
    private Color _foregroundColor = Colors.Black;
    private Color _tapColor = Colors.Black;

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

    public Color ForegroundColor
    {
        get => _foregroundColor;
        set 
        {
            _foregroundColor = value;
            OnPropertyChanged(nameof(ForegroundColor));
        }
    }

    public Color TapColor
    {
        get => _tapColor;
        set
        {
            _tapColor = value;
            OnPropertyChanged(nameof(TapColor));
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

#if ANDROID
        if (this.Handler?.PlatformView is global::Android.Views.View aview)
            aview.Elevation = 4;
#endif
    }

    private void CollapsedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateMenuItems(_view);
    }

    private void OnBackButton()
    {
        if (backButtonBehavior?.OverrideSoftwareBackButtonAction(_context) == true)
            return;

        if (_context is Scaffold scaffold)
            scaffold.SoftwareBackButtonInternal();
    }

    private void OnMenuButton()
    {
        if (_view.GetContext() is Scaffold scaffold)
            scaffold.ShowCollapsedMenusInternal(_view);
    }

    public async Task UpdateVisual(NavigatingArgs e)
    {
        if (e.NavigationType == NavigatingTypes.Push)
        {
            UpdateSafeArea(Scaffold.SafeArea);
            UpdateBackButtonVisual(e.HasBackButton);
            UpdateNavigationBarBackgroundColor(e.NavigationBarBackgroundColor);
            UpdateNavigationBarForegroundColor(e.NavigationBarForegroundColor);
        }

        if (!e.IsAnimating)
            return;

        switch (e.NavigationType)
        {
            case NavigatingTypes.Push:
                this.Opacity = 0;
                await this.Dispatcher.DispatchAsync(async () =>
                {
                    Parallel.Invoke(() =>
                    {
                        this.FadeTo(1, Scaffold.AnimationTime);
                    });
                    await Task.Delay(Scaffold.AnimationTime);
                });
                break;
            case NavigatingTypes.Pop:
                await this.FadeTo(0, Scaffold.AnimationTime);
                break;
            default:
                break;
        }
    }

    public void Dispose()
    {
        if (menuItems != null)
        {
            menuItems.CollapsedItems.CollectionChanged -= CollapsedItems_CollectionChanged;
            menuItems.Dispose();
        }
    }

    private void UpdateTitle(View view)
    {
        string? title = Scaffold.GetTitle(view);
        UpdateTitle(title);
    }

    public void UpdateTitle(string? title)
    {
        labelTitle.Text = title;
    }

    public void UpdateMenuItems(View view)
    {
        if (menuItems != null)
        {
            menuItems.CollapsedItems.CollectionChanged -= CollapsedItems_CollectionChanged;
            menuItems.Dispose();
        }

        menuItems = Scaffold.GetMenuItems(view);
        menuItems.CollapsedItems.CollectionChanged += CollapsedItems_CollectionChanged;
        bool colapseVisible = menuItems.CollapsedItems.Count > 0;

        BindableLayout.SetItemsSource(stackMenu, menuItems.VisibleItems);
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

    public void UpdateNavigationBarBackgroundColor(Color color)
    {
        BackgroundColor = color;

        Color tapColor;
        if (color.IsDark())
            tapColor = Color.FromRgba(255, 255, 255, 200);
        else
            tapColor = Color.FromRgba(100, 100, 100, 100);

        backButton.TapColor = tapColor;
        buttonMenu.TapColor = tapColor;
        TapColor = tapColor;
    }

    public void UpdateNavigationBarForegroundColor(Color color)
    {
        imageBackButton.TintColor = color;
        labelTitle.TextColor = color;
        imageMenu.TintColor = color;
        ForegroundColor = color;
    }

    public void UpdateSafeArea(Thickness safeArea)
    {
        Padding = new Thickness(safeArea.Left, safeArea.Top, safeArea.Right, 0);
    }
}