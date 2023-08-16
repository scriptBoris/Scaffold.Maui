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

    private async void Button_Clicked_1(object sender, EventArgs e)
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
}