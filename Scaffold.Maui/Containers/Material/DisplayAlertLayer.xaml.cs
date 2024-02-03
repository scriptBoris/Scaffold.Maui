using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class DisplayAlertLayer : IDisplayAlert
{
	private readonly TaskCompletionSource<bool> _taskCompletionSource = new();
	private bool? prepareResult;

    public event VoidDelegate? DeatachLayer;

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
        DeatachLayer?.Invoke();
    }

	public Task<bool> GetResult()
	{
		return _taskCompletionSource.Task;
	}

    public async Task OnShow(CancellationToken cancel)
    {
        await this.FadeTo(1, 180);
    }

    public async Task OnHide(CancellationToken cancel)
    {
        await this.FadeTo(0, 180);
    }

    public void OnRemoved()
    {
        _taskCompletionSource.TrySetResult(prepareResult ?? false);
    }
}