using Sample.Controls;
using ScaffoldLib.Maui;
using System.Windows.Input;

namespace Sample.Views.Pizza;

public partial class MasterView
{
	private List<Scaffold> navigations = new();
	private MenuButton? selectedMenuButton;
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
				case 1:
					view = new AccountView();
					break;
				case 2:
					view = new InfoView();
					break;
				case 3:
					view = new SupportView();
					break;
				case 4:
					view = new SettingsView();
					break;
				default:
					throw new ArgumentException();
			}
			nav = new Scaffold();
			nav.AutomationId = menuId.ToString();
			nav.PushAsync(view).ConfigureAwait(false);
			navigations.Add(nav);
		}

		if (selectedMenuButton != null)
			selectedMenuButton.IsSelected = false;

        var selected = stackMenu[menuId] as MenuButton;
        selected!.IsSelected = true;

		selectedMenuButton = selected;
		Detail = nav;

#if !WINDOWS
		IsPresented = false;
#endif
	}
}