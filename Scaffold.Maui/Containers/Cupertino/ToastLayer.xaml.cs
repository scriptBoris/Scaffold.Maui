using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;

namespace ScaffoldLib.Maui.Containers.Cupertino;

public partial class ToastLayer : IToast
{
    private readonly TaskCompletionSource<bool> _tsc = new();
    public event VoidDelegate? DeatachLayer;

    public ToastLayer(CreateToastArgs args)
	{
		InitializeComponent();
        labelTitle.Text = args.Title;
        labelTitle.IsVisible = args.Title != null;
        labelMessage.Text = args.Message;
        Opacity = 0;

        this.Dispatcher.StartTimer(args.ShowTime, () =>
        {
            DeatachLayer?.Invoke();
            return false;
        });
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
           },
           length: 180,
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
           cancel: cancel);
    }

    public void OnShow()
    {
        Opacity = 1;
    }

    public void OnHide()
    {
        Opacity = 0;
    }

    public void OnRemoved()
    {
        _tsc.TrySetResult(true);
    }

    public Task GetResult()
    {
        return _tsc.Task;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        DeatachLayer?.Invoke();
    }

    public void OnTapToOutside()
    {
    }
}