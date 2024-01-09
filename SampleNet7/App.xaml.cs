using SampleDll.Services;

namespace SampleNet7;

public partial class App : Application
{
    public App(IAuthService authService)
    {
        InitializeComponent();
        SampleDll.SampleDllInit.EntryPoint(authService);
    }
}
