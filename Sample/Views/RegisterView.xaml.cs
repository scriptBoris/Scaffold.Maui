using ScaffoldLib.Maui;

namespace Sample.Views;

public partial class RegisterView
{
	public RegisterView()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		this.GetContext()?.PushAsync(new ConfirmEmailView());
    }
}