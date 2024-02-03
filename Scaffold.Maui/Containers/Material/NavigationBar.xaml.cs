using Microsoft.Maui.Controls;
using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using MenuItemCollection = ScaffoldLib.Maui.Core.MenuItemCollection;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class NavigationBar : INavigationBar, IDisposable
{
    private readonly IAgent _agent;
    private readonly View _view;
    private readonly IScaffold _context;
    private MenuItemCollection? menuItems;
    private IBackButtonBehavior? backButtonBehavior;
    private Color _foregroundColor = Colors.Black;
    private Color _tapColor = Colors.Black;

    public NavigationBar(CreateNavigationBarArgs args)
    {
        _view = args.View;
        _agent = args.Agent;
        _context = args.Agent.Context;
        InitializeComponent();
        backButton.TapCommand = new Command(OnBackButton);
        buttonMenu.TapCommand = new Command(OnMenuButton);
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

    private void CollapsedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        int count = menuItems?.CollapsedItems?.Count ?? 0;
        buttonMenu.IsVisible = count > 0;
    }

    private void OnBackButton()
    {
        _agent.OnBackButton();
    }

    private void OnMenuButton()
    {
        _agent.OnMenuButton();
    }

    public void UpdateTitle(string? title)
    {
        labelTitle.Text = title;
    }

    public void UpdateMenuItems(Core.MenuItemCollection menu)
    {
        if (menuItems != null)
        {
            menuItems.CollapsedItems.CollectionChanged -= CollapsedItems_CollectionChanged;
            menuItems.Dispose();
        }

        menuItems = menu;
        menuItems.CollapsedItems.CollectionChanged += CollapsedItems_CollectionChanged;
        bool colapseVisible = menuItems.CollapsedItems.Count > 0;

        BindableLayout.SetItemsSource(stackMenu, menuItems.VisibleItems);
        buttonMenu.IsVisible = colapseVisible;
    }

    public void UpdateNavigationBarVisible(bool visible)
    {
        IsVisible = visible;
    }

    public void UpdateBackButtonVisibility(bool isVisible)
    {
        var src = backButtonBehavior?.OverrideBackButtonIcon(_agent, _context);
        imageBackButton.Source = src ?? new SvgImageSource("ic_arrow_left.svg");

        var visible = backButtonBehavior?.OverrideBackButtonVisibility(_agent, _context);
        backButton.IsVisible = visible ?? isVisible;
    }

    public void UpdateBackButtonBehavior(IBackButtonBehavior? behavior)
    {
        backButtonBehavior = behavior;
        UpdateBackButtonVisibility(backButton.IsVisible);
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

    public void Dispose()
    {
        if (menuItems != null)
        {
            menuItems.CollapsedItems.CollectionChanged -= CollapsedItems_CollectionChanged;
            menuItems.Dispose();
            menuItems = null;
        }

        var all = this.GetDeepAllChildren();
        foreach (var item in all)
        {
            if (item is IDisposable disposable)
                disposable.Dispose();
        }
        Handler = null;
    }
}