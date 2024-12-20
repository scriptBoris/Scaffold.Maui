using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Toolkit;
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

    public Task OnShow(CancellationToken cancel)
    {
        return this.AnimateTo(
            start: Opacity,
            end: 1,
            name: nameof(OnShow),
            updateAction: (v, value) => v.Opacity = value,
            length: 180,
            cancel: cancel
        );
    }

    public Task OnHide(CancellationToken cancel)
    {
        return this.AnimateTo(
            start: Opacity,
            end: 0,
            name: nameof(OnHide),
            updateAction: (v, value) => v.Opacity = value,
            length: 180,
            cancel: cancel
        );
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

    public void OnTapToOutside()
    {
    }
}