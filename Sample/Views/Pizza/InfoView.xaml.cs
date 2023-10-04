namespace Sample.Views.Pizza;

public partial class InfoView
{
	public InfoView()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		var label = sender as Label;
		Browser.OpenAsync(label.Text);
    }
}