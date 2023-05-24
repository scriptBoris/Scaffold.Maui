using ScaffoldLib.Maui;
using ScaffoldLib.Maui.Core;

namespace Sample.Views;

public partial class NodeView : IAppear, IDisappear
{
	public NodeView()
	{
		InitializeComponent();
        BindingContext = new NodeViewModel(this);
	}

    public void OnAppear(bool isComplete)
    {
        System.Diagnostics.Debug.WriteLine($"NodeView.Appearing:isComplete={isComplete}");
    }

    public void OnDisappear(bool isComplete)
    {
        System.Diagnostics.Debug.WriteLine($"NodeView.Disappearing:isComplete={isComplete}");
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