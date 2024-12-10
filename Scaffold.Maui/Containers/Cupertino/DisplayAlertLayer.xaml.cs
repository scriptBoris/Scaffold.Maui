using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;
using System.Threading.Channels;

namespace ScaffoldLib.Maui.Containers.Cupertino;

public partial class DisplayAlertLayer : IDisplayAlert
{
    public event VoidDelegate? DeatachLayer;
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();
    private bool? prepareResult;
    private double _showProgress;

    public DisplayAlertLayer(ICreateDisplayAlertArgs args)
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

    public Task OnShow(CancellationToken cancel)
    {
        return this.AnimateTo(
            start: _showProgress,
            end: 1,
            name: nameof(OnShow),
            updateAction: (v, value) =>
            {
                _showProgress = value;
                v.Opacity = value;
                v.Scale = double.Lerp(1.4, 1, Easing.CubicInOut.Ease(value));
            },
            length: 180,
            cancel: cancel);
    }

    public Task OnHide(CancellationToken cancel)
    {
        return this.AnimateTo(
            start: _showProgress,
            end: 0,
            name: nameof(OnHide),
            updateAction: (v, value) =>
            {
                _showProgress = value;
                v.Opacity = value;
            },
            length: 180,
            cancel: cancel);
    }

    public void OnShow()
    {
        Opacity = 1;
        Scale = 1;
    }

    public void OnHide()
    {
        Opacity = 0;
        Scale = 1.4;
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