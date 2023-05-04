using Sample.Views;

namespace Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var mainPage = new MainPage();
            mainPage.Scaffold.PushAsync(new LoginView());

            MainPage = mainPage;
        }
    }
}