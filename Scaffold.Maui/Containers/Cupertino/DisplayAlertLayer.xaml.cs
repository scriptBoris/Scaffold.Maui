using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using System.Threading.Channels;

namespace ScaffoldLib.Maui.Containers.Cupertino;

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
        Scale = 1.4;
        GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => Close(false)),
        });

        buttonOk.TapCommand = new Command(() => Close(true));
        buttonCancel.TapCommand = new Command(() => Close(false));
        labelTitle.Text = args.Title;
        labelDescription.Text = args.Description;
        labelButtonOk.Text = args.Ok;

        if (args.Cancel == null)
        {
            buttonCancel.IsVisible = false;
            lineButtons.IsVisible = false;
        }
        else
        {
            labelButtonCancel.Text = args.Cancel;
        }
    }

    public Task<bool> GetResult()
    {
        return _taskCompletionSource.Task;
    }

    public async Task OnShow(CancellationToken cancel)
    {
        this.CancelAnimations();
        await Task.WhenAll(
            this.FadeTo(1, 180),
            this.ScaleTo(1, 180, Easing.CubicInOut)
        );
    }

    public async Task OnHide(CancellationToken cancel)
    {
        this.CancelAnimations();
        await this.FadeTo(0, 180);
    }

    public void OnRemoved()
    {
        _taskCompletionSource.TrySetResult(prepareResult ?? false);
    }

    private void Close(bool result)
    {
        prepareResult ??= result;
        DeatachLayer?.Invoke();
    }

}