using SamplePizza.ViewModels;

namespace SamplePizza;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        var mainPage = new MainPage();
        var scaffold = mainPage.Scaffold;
        scaffold.PushAsync(new LoginViewModel().View);
        MainPage = mainPage;
    }
}