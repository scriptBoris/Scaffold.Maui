using ScaffoldLib.Maui;
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

    public ICommand CommandToast => new Command(() =>
    {
        this.GetContext()?.Toast("Hello", "Toast message", TimeSpan.FromSeconds(4));
    });

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.GetContext()?.PopToRootAsync();
    }
}