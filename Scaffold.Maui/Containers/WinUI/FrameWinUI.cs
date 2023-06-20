using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Toolkit.FlyoutViewPlatforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.WinUI;

public class FrameWinUI : Frame
{
    private readonly IScaffold _context;

    public FrameWinUI(View view, IScaffold context) : base(view, context)
    {
        _context = context;
        _context.ExternalBevahiors.AsNotifyObs().CollectionChanged += WinUIFrame_CollectionChanged;
        var flyout = _context.ExternalBevahiors.FirstOrDefault(x => x is FlyoutViewWinUI.FlyoutBehavior) as FlyoutViewWinUI.FlyoutBehavior;
        if (flyout != null)
            flyout.Changed += Flyout_Changed;
    }

    private void Flyout_Changed(FlyoutViewWinUI.FlyoutBehavior flyout)
    {
        if (NavigationBar is Layout nav)
            nav.Padding = new Thickness(flyout.NavigationBarOffset, 0, 0, 0);

        if (ViewWrapper is Layout wrapper)
            wrapper.Margin = new Thickness(flyout.ViewOffset, 0, 0, 0);
    }

    private void WinUIFrame_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        object item;
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                item = e.NewItems![0]!;
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                item = e.OldItems![0]!;
                break;
            default:
                return;
        }

        if (item is FlyoutViewWinUI.FlyoutBehavior)
            UpdateFlyoutOffsets();
    }

    private void UpdateFlyoutOffsets()
    {
        var flyout = _context.ExternalBevahiors.FirstOrDefault(x => x is FlyoutViewWinUI.FlyoutBehavior) as FlyoutViewWinUI.FlyoutBehavior;
        if (flyout != null)
            Flyout_Changed(flyout);
    }

    public override void DrawLayout()
    {
        base.DrawLayout();
        UpdateFlyoutOffsets();
    }
}
