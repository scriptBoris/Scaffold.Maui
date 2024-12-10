using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class ToastLayer : IToast
{
    private readonly TaskCompletionSource<bool> _tsc = new();

    public event VoidDelegate? DeatachLayer;
    private bool isInitialized;

    public ToastLayer(CreateToastArgs args)
	{
		InitializeComponent();
        labelTitle.Text = args.Title;
        labelTitle.IsVisible = args.Title != null;
        labelMessage.Text = args.Message;
        this.Dispatcher.StartTimer(args.ShowTime, () =>
        {
            DeatachLayer?.Invoke();
            return false;
        });
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var res = base.MeasureOverride(widthConstraint, heightConstraint);

        if (!isInitialized)
        {
            isInitialized = true;
            frame.TranslationY = frame.DesiredSize.Height;
        }
        return res;
    }

    public async Task OnShow(CancellationToken cancellation)
    {
        await frame.TranslateTo(0, 0, 140, Easing.CubicIn);
    }

    public async Task OnHide(CancellationToken cancellation)
    {
        await this.FadeTo(0, 180);
    }

    public void OnShow()
    {
        Opacity = 1;
        frame.TranslationX = 0;
        frame.TranslationY = 0;
    }

    public void OnHide()
    {
        Opacity = 0;
        frame.TranslationY = frame.Height;
    }

    public Task GetResult()
    {
        return _tsc.Task;
    }

    public void OnRemoved()
    {
        _tsc.TrySetResult(true);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        DeatachLayer?.Invoke();
    }
}