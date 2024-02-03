using SampleDll.Services;
using System.Diagnostics;
using System.Reflection;

namespace SampleNet8
{
    public partial class App : Application
    {
        public App(IAuthService authService)
        {
            InitializeComponent();
            SampleDll.SampleDllInit.EntryPoint(authService);
        }
    }
}
