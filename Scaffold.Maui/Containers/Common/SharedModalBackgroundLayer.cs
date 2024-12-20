using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;

namespace ScaffoldLib.Maui.Containers.Common;

public class SharedModalBackgroundLayer : Grid, ISharedModalBackground
{
    public event VoidDelegate? DeatachLayer;
    public event SharedModalBackgroundTapped? TappedToOutside;
    private readonly TapGestureRecognizer _tapGestureRecognizer;

    public SharedModalBackgroundLayer()
    {
        BackgroundColor = Color.FromRgba(0, 0, 0, 100);
        Opacity = 0;

        _tapGestureRecognizer = new TapGestureRecognizer();
        _tapGestureRecognizer.Tapped += _tapGestureRecognizer_Tapped;
        GestureRecognizers.Add(_tapGestureRecognizer);
    }

    public int ZBufferIndex { get; set; }

    private void _tapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        TappedToOutside?.Invoke(this, e);
    }

    public void OnHide()
    {
        Opacity = 0;
    }

    public Task OnHide(CancellationToken cancel)
    {
        return this.AnimateTo(
            start: Opacity,
            end: 0,
            name: nameof(OnHide),
            updateAction: (v, value) => v.Opacity = value,
            length: 180,
            cancel: cancel);
    }

    public void OnShow()
    {
        Opacity = 1;
    }

    public Task OnShow(CancellationToken cancel)
    {
        return this.AnimateTo(
            start: Opacity,
            end: 1,
            name: nameof(OnHide),
            updateAction: (v, value) => v.Opacity = value,
            length: 180,
            cancel: cancel);
    }

    public void OnRemoved()
    {
    }
}