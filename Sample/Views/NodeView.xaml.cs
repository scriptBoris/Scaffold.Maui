using ScaffoldLib.Maui;
using ScaffoldLib.Maui.Core;

namespace Sample.Views;

public partial class NodeView
{
	public NodeView()
	{
		InitializeComponent();
        BindingContext = new NodeViewModel(this);
	}
}

public class NodeViewModel : IBackButtonListener
{
    private readonly View view;

    public NodeViewModel(View view)
    {
        this.view = view;
    }

    public async Task<bool> OnBackButton()
    {
        bool res = await view.GetContext()!.DisplayAlert("Question", "You are sure to back?", "Back", "Cancel");
        return res;
    }
}