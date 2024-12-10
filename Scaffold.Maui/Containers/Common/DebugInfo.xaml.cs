using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers.Common;

public partial class DebugInfo : IZBufferLayout
{
	public DebugInfo()
	{
		InitializeComponent();
	}

    public event VoidDelegate? DeatachLayer;

    public Task OnShow(CancellationToken cancel)
    {
        return Task.CompletedTask;
    }

    public Task OnHide(CancellationToken cancel)
    {
        return Task.CompletedTask;
    }

    public void OnShow()
    {
    }

    public void OnHide()
    {
    }

    public void OnRemoved()
    {
    }
}