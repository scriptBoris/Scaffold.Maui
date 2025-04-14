using SampleDll.Views.Popups;
using ScaffoldLib.Maui;

namespace SampleDll.Views;

public partial class MasterView
{
    public MasterView()
	{
		InitializeComponent();


        this.Dispatcher.StartTimer(TimeSpan.FromSeconds(5), () =>
        {
            var root = Scaffold.GetRootScaffold();
            root.AddCustomLayer(new UpdatePopup(), IScaffold.PopupIndexZ);
            return false;
        });
    }
}