using ScaffoldLib.Maui;

namespace SampleDll;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public IScaffold Scaffold => scaffold;
}