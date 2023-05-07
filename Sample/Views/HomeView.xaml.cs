using Scaffold.Maui;

namespace Sample.Views;

public partial class HomeView
{
	public HomeView()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		this.GetContext()?.PushAsync(new NodeView());
    }
}