using Microsoft.Maui.Controls;
using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using System.Windows.Input;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class NavigationBar : INavigationBar, IDisposable
{
    private readonly IAgent _agent;
    private readonly View _view;
    private readonly IScaffold _context;
    private IBackButtonBehavior? backButtonBehavior;
    private Color _foregroundColor = Colors.Black;
    private Color _tapColor = Colors.Black;
    private Thickness _defaultPadding = new Thickness(7, 0);
    private View? _titleView;

    public NavigationBar(CreateNavigationBarArgs args)
    {
        _view = args.View;
        _agent = args.Agent;
        _context = args.Agent.Context;
        Padding = _defaultPadding;
        InitializeComponent();
        backButton.TapCommand = new Command(OnBackButton);
        CommandMenu = new Command(OnMenuButton);
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

    public ICommand CommandMenu { get; }

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

    public void UpdateMenuItems(IList<ScaffoldMenuItem>? menu)
    {
        menuItemsLayout.ItemsSource = menu;
    }

    public void UpdateNavigationBarVisible(bool visible)
    {
        IsVisible = visible;
    }

    public void UpdateBackButtonVisibility(bool isVisible)
    {
        var src = backButtonBehavior?.OverrideBackButtonIcon(_agent, _context);
        imageBackButton.Source = src ?? ImageSource.FromFile("scaffoldlib_arrow_left.png");

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
        TapColor = tapColor;
    }

    public void UpdateNavigationBarForegroundColor(Color color)
    {
        imageBackButton.TintColor = color;
        labelTitle.TextColor = color;
        ForegroundColor = color;
    }

    public void UpdateSafeArea(Thickness safeArea)
    {
        var safeAreaPadding = new Thickness(safeArea.Left, safeArea.Top, safeArea.Right, 0);
        Padding = _defaultPadding + safeAreaPadding;
    }

    public void UpdateTitleView(View? titleView)
    {
        var old = _titleView;
        var nev = titleView;

        // ignore
        if (old == nev)
            return;

        if (old != null)
        {
            Children.Remove(_titleView);
            _titleView = null;
        }
        
        if (nev != null)
        {
            Grid.SetColumn(nev, 1);
            Children.Add(nev);
            _titleView = nev;
            _titleView.BindingContext = _view.BindingContext;
        }

        labelTitle.IsVisible = _titleView == null;
    }

    public void Dispose()
    {
        var all = this.GetDeepAllChildren();
        foreach (var item in all)
        {
            if (item is IDisposable disposable)
                disposable.Dispose();
        }
        Handler = null;
    }
}