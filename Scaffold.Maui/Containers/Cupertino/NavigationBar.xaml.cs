using Microsoft.Maui.Controls;
using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui.Containers.Cupertino;

public partial class NavigationBar : INavigationBar, IDisposable
{
    private readonly View _view;
    private readonly IAgent _agent;
    private readonly IScaffold _context;
    private Color _foregroundColor = Colors.Black;
    private Color _tapColor = Colors.Black;
    private IBackButtonBehavior? backButtonBehavior;
    private ScaffoldMenuItems? menuItems;

    public NavigationBar(CreateNavigationBarArgs args)
	{
        _view = args.View;
        _agent = args.Agent;
        _context = args.Agent.Context;
        IgnoreSafeArea = true;
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

    private void OnBackButton()
    {
        _agent.OnBackButton();
    }

    private void OnMenuButton()
    {
        _agent.OnMenuButton();
    }

    public void UpdateBackButtonBehavior(IBackButtonBehavior? behavior)
    {
        backButtonBehavior = behavior;
        UpdateBackButtonVisibility(backButton.IsVisible);
    }

    public void UpdateBackButtonVisibility(bool isVisible)
    {
        var src = backButtonBehavior?.OverrideBackButtonIcon(_agent, _context);
        backButton.ImageSource = src ?? new SvgImageSource("scaffoldlib_arrow_left.svg", 44,44);

        var visible = backButtonBehavior?.OverrideBackButtonVisibility(_agent, _context);
        backButton.IsVisible = visible ?? isVisible;
    }

    public void UpdateMenuItems(IList<ScaffoldMenuItem>? menu)
    {
        // TODO Использовать продвинутыйы MenuItemsLayout вместо ручной реализации на каждой платформе
        //menuItems = menu;
        //menuItems.CollapsedItems.CollectionChanged += CollapsedItems_CollectionChanged;
    }

    public void UpdateNavigationBarBackgroundColor(Color color)
    {
        BackgroundColor = color;

        Color tapColor = Color.FromArgb("#6da9d8");
        //if (color.IsDark())
        //    tapColor = Color.FromRgba(255, 255, 255, 200);
        //else
        //    tapColor = Color.FromRgba(100, 100, 100, 100);

        backButton.TapColor = tapColor;
        buttonMenu.TapColor = tapColor;
        TapColor = tapColor;
    }

    public void UpdateNavigationBarForegroundColor(Color color)
    {
        backButton.ForegroundColor = color;
        labelTitle.TextColor = color;
        buttonMenu.ForegroundColor = color;
        ForegroundColor = color;
    }

    public void UpdateNavigationBarVisible(bool visible)
    {
        IsVisible = visible;
    }

    public void UpdateSafeArea(Thickness safeArea)
    {
        Padding = new Thickness(safeArea.Left, safeArea.Top, safeArea.Right, 0);
    }

    public void UpdateTitle(string? title)
    {
        labelTitle.Text = title;
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

    public void UpdateTitleView(View? titleView)
    {
        // TODO Not implement
        throw new NotImplementedException();
    }
}