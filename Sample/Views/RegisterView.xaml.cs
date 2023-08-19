using ScaffoldLib.Maui;

namespace Sample.Views;

public partial class RegisterView
{
    public RegisterView()
    {
        InitializeComponent();
    }

    private void Accept(object sender, EventArgs e)
    {
        this.GetContext()?.PushAsync(new ConfirmEmailView());
    }

    private async void AcceptLatency(object sender, EventArgs e)
    {
        await Task.Delay(5000);
        Accept(sender, e);
    }

    private async void SelectLanguage(object sender, EventArgs e)
    {
        var context = this.GetContext();
        if (sender is Button button && context != null)
        {
            var res = await context.DisplayActionSheet("Select language", "Cancel", "Clear",
                "English",
                "Russian",
                "Urkaine",
                "Kazakhstan",
                "Uzbekistan",
                "Spanish",
                "Italian",
                "Chinese",
                "Japanese"
                );

            if (res.HasSelectedItem)
                button.Text = res.ItemText;

            if (res.IsDestruction)
                button.Text = "Select language";
        }
    }

    private void ShowDialog(object sender, EventArgs e)
    {
        this.GetContext()?.DisplayAlert("Hello", "Message", "Cancel", this);
    }

    private async void BackLatency(object sender, EventArgs e)
    {
        await Task.Delay(5000);
        this.GetContext()?.PopAsync();
    }
}