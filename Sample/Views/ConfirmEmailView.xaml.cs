using Scaffold.Maui;
using System.Windows.Input;

namespace Sample.Views;

public partial class ConfirmEmailView
{
	public ConfirmEmailView()
	{
		InitializeComponent();
		BindingContext = this;
	}

	public ICommand CommandRetry => new Command(() =>
	{
		this.GetContext()?.DisplayAlert("Title", "Description", "Retry", "Cancel");
	});

    private void Button_Clicked(object sender, EventArgs e)
    {
		this.GetContext()?.PopToRootAsync();
    }
}