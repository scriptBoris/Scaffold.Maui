using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class DisplayAlertLayer : IDisplayAlert
{
    public event VoidDelegate? DeatachLayer;
	private readonly TaskCompletionSource<bool> _taskCompletionSource = new();
	private bool isBusy;
	private bool? prepareResult;

    public DisplayAlertLayer(CreateDisplayAlertArgs args)
    {
        InitializeComponent();
        Opacity = 0;
        GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => Close(false)),
        });

        buttonOk.TapCommand = new Command(() => Close(true));
        buttonCancel.TapCommand = new Command(() => Close(false));

        labelTitle.Text = args.Title;
        labelDescription.Text = args.Description;
        labelButtonOk.Text = args.Ok;

        // single button
        if (args.Cancel == null)
        {
            buttonCancel.IsVisible = false;
        }
        // two button
        else
        {
            labelButtonCancel.Text = args.Cancel;
        }
    }

    private void Close(bool result)
    {
        prepareResult ??= result;
		Close().ConfigureAwait(false);
    }

	public Task<bool> GetResult()
	{
		return _taskCompletionSource.Task;
	}

    public async Task Show()
    {
		isBusy = true;
        await this.FadeTo(1, 180);
		isBusy = false;
    }

    public async Task Close()
    {
        if (isBusy)
            return;

        isBusy = true;

        await this.FadeTo(0, 180);
        _taskCompletionSource.TrySetResult(prepareResult ?? false);
		DeatachLayer?.Invoke();
    }
}