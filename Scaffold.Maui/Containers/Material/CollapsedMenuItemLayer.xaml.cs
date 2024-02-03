using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class CollapsedMenuItemLayer : IZBufferLayout
{
    public event VoidDelegate? DeatachLayer;

    public CollapsedMenuItemLayer(CreateCollapsedMenuArgs args)
	{
		InitializeComponent();
        Padding = Scaffold.DeviceSafeArea;
        Opacity = 0;
        CommandSelectedMenu = new Command(ActionSelectedMenu);
        BindingContext = this;
        GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => DeatachLayer?.Invoke()),
        });

        var obs = Scaffold.GetMenuItems(args.View).CollapsedItems;
        BindableLayout.SetItemsSource(stackMenu, obs);
	}

    public ICommand CommandSelectedMenu { get; private set; }

    private void ActionSelectedMenu(object param)
    {
        if (param is ScaffoldMenuItem menuItem)
        {
            menuItem.Command?.Execute(null);
        }
        DeatachLayer?.Invoke();
    }

    public async Task OnShow(CancellationToken cancel)
    {
        await this.FadeTo(1, 180);
    }

    public async Task OnHide(CancellationToken cancel)
    {
        await this.FadeTo(0, 180);
    }

    public void OnRemoved()
    {
    }
}