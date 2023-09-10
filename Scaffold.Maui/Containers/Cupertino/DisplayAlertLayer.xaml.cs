using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers.Cupertino;

public partial class DisplayAlertLayer : IDisplayAlert
{
    public event VoidDelegate? DeatachLayer;
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();
    private bool isBusy;
    private bool? prepareResult;

    private DisplayAlertLayer()
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
    }

    public DisplayAlertLayer(string title, string description, string ok) : this()
    {
        labelTitle.Text = title;
        labelDescription.Text = description;
        labelButtonOk.Text = ok;
        buttonCancel.IsVisible = false;
        lineButtons.IsVisible = false;
    }

    public DisplayAlertLayer(string title, string description, string ok, string cancel) : this()
    {
        labelTitle.Text = title;
        labelDescription.Text = description;
        labelButtonOk.Text = ok;
        labelButtonCancel.Text = cancel;
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

        await Task.WhenAll(
            this.FadeTo(1, 180),
            this.ScaleTo(1, 180, Easing.CubicInOut)
        );

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