using Scaffold.Maui;
using System.Windows.Input;

namespace Sample.Views;

public partial class MasterView
{
	private List<ScaffoldView> navigations = new();

	public MasterView()
	{
		InitializeComponent();
		BindingContext = this;
		SelectMenu(0);
    }

	public ICommand CommandSelectMenu => new Command((param) =>
	{
		if (param is int menuId)
			SelectMenu(menuId);

		if (param is string menuIdString && int.TryParse(menuIdString, out int parseId))
            SelectMenu(parseId);
    });

	private void SelectMenu(int menuId)
	{
		var nav = navigations.FirstOrDefault(x => x.AutomationId == menuId.ToString());
		if (nav == null)
		{
			View? view = null;
			switch (menuId)
			{
				case 0:
					view = new HomeView();
					break;
				default:
					throw new ArgumentException();
			}
			nav = new ScaffoldView();
			nav.PushAsync(view);
			navigations.Add(nav);
		}
		Detail = nav;
    }
}