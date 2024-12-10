using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScaffoldLib.Maui.Containers.WinUI;

public partial class CollapsedMenuItemLayer : IZBufferLayout
{
    private bool isBusy;

    public event VoidDelegate? DeatachLayer;

    public CollapsedMenuItemLayer(CreateCollapsedMenuArgs args)
	{
		InitializeComponent();
        CommandSelectedMenu = new Command(ActionSelectedMenu);
        BindingContext = this;
        GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => DeatachLayer?.Invoke())
        });
        Opacity = 0;

        var obs = Scaffold.GetMenuItems(args.View).CollapsedItems;
        BindableLayout.SetItemsSource(stackMenu, obs);
	}

    public ICommand CommandSelectedMenu { get; private set; }

    private void ActionSelectedMenu(object param)
    {
        if (!isBusy && param is ScaffoldMenuItem menuItem)
        {
            menuItem.Command?.Execute(null);
        }
        DeatachLayer?.Invoke();
    }

    public async Task OnShow(CancellationToken cancel)
    {
        isBusy = true;
        await this.FadeTo(1, 180);
        isBusy = false;
    }

    public async Task OnHide(CancellationToken cancel)
    {
        isBusy = true;
        await this.FadeTo(0, 180);
        isBusy = false;
    }

    public void OnShow()
    {
        Opacity = 1;
    }

    public void OnHide()
    {
        Opacity = 0;
    }

    public void OnRemoved()
    {
    }
}