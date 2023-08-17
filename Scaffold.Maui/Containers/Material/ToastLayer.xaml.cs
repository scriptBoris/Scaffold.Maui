using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class ToastLayer : IToast
{
    private readonly TaskCompletionSource<bool> _tsc = new();
    private bool isAnimationClose;
    private bool isAnimationShow;

	public ToastLayer(string? title, string message, TimeSpan showTime)
	{
		InitializeComponent();
        labelTitle.Text = title;
        labelTitle.IsVisible = title != null;
        labelMessage.Text = message;
        this.Dispatcher.StartTimer(showTime, () =>
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