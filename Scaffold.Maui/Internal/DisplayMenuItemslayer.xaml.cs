using Scaffold.Maui.Core;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scaffold.Maui.Internal;

public partial class DisplayMenuItemslayer : IZBufferLayout
{
    private bool isBusy;

    public event VoidDelegate? DeatachLayer;

    public DisplayMenuItemslayer(View view)
	{
		InitializeComponent();
        CommandSelectedMenu = new Command(ActionSelectedMenu);
        BindingContext = this;

        var obs = ScaffoldView.GetMenuItems(view).CollapsedItems;
        BindableLayout.SetItemsSource(stackMenu, obs);
	}

    public ICommand CommandSelectedMenu { get; private set; }

    private void ActionSelectedMenu(object param)
    {
        if (param is MenuItem menuItem)
        {
            menuItem.Command?.Execute(null);
        }
        Close().ConfigureAwait(false);
    }

    public async Task Show()
    {
        isBusy = true;
        await this.FadeTo(1, 180);
        isBusy = false;
    }

    public async Task Close()
    {
        if (isBusy)
            return;

        isBusy = true;

        await this.FadeTo(0, 180);
        DeatachLayer?.Invoke();
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Close().ConfigureAwait(false);
    }
}