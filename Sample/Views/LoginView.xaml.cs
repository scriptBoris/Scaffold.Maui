using Scaffold.Maui;

namespace Sample.Views;

public partial class LoginView
{
	public LoginView()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		this.GetContext()?.PushAsync(new MasterView());
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        this.GetContext()?.PushAsync(new RegisterView());
    }
}