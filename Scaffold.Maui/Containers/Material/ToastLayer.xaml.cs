using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class ToastLayer : IToast
{
    private readonly TaskCompletionSource<bool> _tsc = new();
    private bool isAnimationClose;
    private bool isAnimationShow;

	public ToastLayer(CreateToastArgs args)
	{
		InitializeComponent();
        labelTitle.Text = args.Title;
        labelTitle.IsVisible = args.Title != null;
        labelMessage.Text = args.Message;
        this.Dispatcher.StartTimer(args.ShowTime, () =>
        {
            _ = Close();
            return false;
        });
    }

    public event VoidDelegate? DeatachLayer;

    public async Task Close()
    {
        if (!isAnimationClose)
        {
            isAnimationClose = true;
            await this.FadeTo(0, 180);
            _tsc.TrySetResult(true);
            DeatachLayer?.Invoke();
        }
    }

    public Task GetResult()
    {
        return _tsc.Task;
    }

    public async Task Show()
    {
        isAnimationShow = true;
        frame.TranslationY = 1000000;
        this.Dispatcher.Dispatch(() =>
        {
            frame.TranslationY = frame.Height;
            Parallel.Invoke(() =>
            {
                new Animation(
                    callback: v => frame.TranslationY = v,
                    start: frame.Height,
                    end: 0)
                .Commit(frame, "show", 13, 140, Easing.CubicIn);
            });
        });
        await Task.Delay(140);
        isAnimationShow = false;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = Close();
    }
}