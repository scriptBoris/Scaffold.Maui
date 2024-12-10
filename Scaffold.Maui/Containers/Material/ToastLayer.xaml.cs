using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Toolkit;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class ToastLayer : IToast
{
    private readonly TaskCompletionSource<bool> _tsc = new();

    public event VoidDelegate? DeatachLayer;
    private double _progressShow = 0;

    public ToastLayer(CreateToastArgs args)
	{
		InitializeComponent();
        labelTitle.Text = args.Title;
        labelTitle.IsVisible = args.Title != null;
        labelMessage.Text = args.Message;
        frame.TranslationY = 1500;
        this.Dispatcher.StartTimer(args.ShowTime, () =>
        {
            DeatachLayer?.Invoke();
            return false;
        });
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        DeatachLayer?.Invoke();
    }

    public Task OnShow(CancellationToken cancellation)
    {
        return frame.AnimateTo(
            start: _progressShow,
            end: 1,
            name: nameof(OnShow),
            updateAction: (view, value) =>
            {
                _progressShow = value;
                if (view.Height >= 0)
                    view.TranslationY = double.Lerp(view.Height, 0, value);
            },
            length: 140,
            easing: Easing.CubicIn,
            cancel: cancellation);
    }

    public Task OnHide(CancellationToken cancellation)
    {
        return frame.AnimateTo(
            start: _progressShow,
            end: 0,
            name: nameof(OnHide),
            updateAction: (view, value) =>
            {
                _progressShow = value;
                if (view.Height >= 0)
                    view.TranslationY = double.Lerp(view.Height, 0, value);
            },
            length: 180,
            cancel: cancellation);
    }

    public void OnShow()
    {
        Opacity = 1;
        frame.TranslationY = 0;
    }

    public void OnHide()
    {
        Opacity = 0;
        frame.TranslationY = 500;
    }

    public Task GetResult()
    {
        return _tsc.Task;
    }

    public void OnRemoved()
    {
        _tsc.TrySetResult(true);
    }
}