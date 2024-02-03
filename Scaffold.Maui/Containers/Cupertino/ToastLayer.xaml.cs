using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;

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
}