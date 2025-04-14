using ScaffoldLib.Maui.Core;

namespace SampleDll.Views.Popups;

public partial class UpdatePopup : IModalLayout, IBackButtonListener
{
	public UpdatePopup()
	{
		InitializeComponent();
	}

    public event VoidDelegate? DeatachLayer;

    public Task<bool> OnBackButton()
    {
        return Task.FromResult(false);
    }

    public void OnHide()
    {
    }

    public Task OnHide(CancellationToken cancel)
    {
        return Task.CompletedTask;
    }

    public void OnRemoved()
    {
    }

    public void OnShow()
    {
    }

    public Task OnShow(CancellationToken cancel)
    {
        return Task.CompletedTask;
    }

    public void OnTapToOutside()
    {
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        DeatachLayer?.Invoke();
    }
}