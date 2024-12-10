using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;

namespace ScaffoldLib.Maui.Containers.Common;

public class AlertZBufferBackgroundLayer : Grid, IZBufferLayout
{
    public event VoidDelegate? DeatachLayer;

    public AlertZBufferBackgroundLayer()
    {
        BackgroundColor = Color.FromRgba(0, 0, 0, 100);
        Opacity = 0;
        InputTransparent = false;
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