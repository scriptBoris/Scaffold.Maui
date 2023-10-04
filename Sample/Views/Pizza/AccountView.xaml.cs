using ScaffoldLib.Maui;
using System.Windows.Input;

namespace Sample.Views.Pizza;

public partial class AccountView
{
	public AccountView()
	{
		InitializeComponent();
		BindingContext = this;
	}

	public ICommand CommandLogout => new Command(() =>
	{
		if (App.Current?.MainPage is MainPage rootPage)
		{
			var rootView = rootPage.Scaffold.NavigationStack.First();
			rootPage.Scaffold.ReplaceView(rootView, new LoginView());
		}
	});

	public ICommand CommandDeleteAccount => new Command(() =>
	{
        if (App.Current?.MainPage is MainPage rootPage)
        {
            var rootView = rootPage.Scaffold.NavigationStack.First();
            rootPage.Scaffold.ReplaceView(rootView, new LoginView());
        }
    });
}