using Scaffold.Maui;

namespace Sample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public ScaffoldView Scaffold => scaffold;
}