using ScaffoldLib.Maui;

namespace Sample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new Scaffold();
    }

    public Scaffold Scaffold => (Scaffold)Content;
}