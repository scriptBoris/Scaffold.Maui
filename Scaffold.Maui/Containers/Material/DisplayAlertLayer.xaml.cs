using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class DisplayAlertLayer : IDisplayAlert
{
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();
    private bool? prepareResult;

    public event VoidDelegate? DeatachLayer;

    public DisplayAlertLayer(ICreateDisplayAlertArgs args)
    {
        InitializeComponent();
        OnHide();

        buttonOk.TapCommand = new Command(() => Close(true));
        buttonCancel.TapCommand = new Command(() => Close(false));

        labelTitle.IsVisible = args.Title != null;
        labelTitle.Text = args.Title;
        labelDescription.IsVisible = args.Description != null;
        labelDescription.Text = args.Description ?? "";
        labelButtonOk.Text = args.Ok;
        specialLayout.BodyLength = labelDescription.Text.Length;

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

    public Task OnShow(CancellationToken cancel)
    {
        return this.AnimateTo(
            start: Opacity,
            end: 1,
            name: nameof(OnShow),
            updateAction: (v, value) =>
            {
                v.Opacity = value;
                v.Scale = double.Lerp(0.95, 1.0, value);
            },
            length: 180,
            easing: null,
            cancel: cancel);
    }

    public Task OnHide(CancellationToken cancel)
    {
        return this.AnimateTo(
            start: Opacity,
            end: 0,
            name: nameof(OnHide),
            updateAction: (v, value) =>
            {
                v.Opacity = value;
            },
            length: 180,
            easing: null,
            cancel: cancel);
    }

    public void OnShow()
    {
        this.Opacity = 1;
        this.Scale = 1;
    }

    public void OnHide()
    {
        this.Opacity = 0;
        this.Scale = 0.95;
    }

    public void OnRemoved()
    {
        _taskCompletionSource.TrySetResult(prepareResult ?? false);
    }

    public void OnTapToOutside()
    {
        DeatachLayer?.Invoke();
    }
}