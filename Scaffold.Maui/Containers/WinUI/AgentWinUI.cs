using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;
using ScaffoldLib.Maui.Toolkit.FlyoutViewPlatforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.WinUI;

public class AgentWinUI : Agent, IWindowsBehavior
{
    private readonly IScaffold _context;
    private readonly StaticLibs.ButtonSam.Button? _flytoutButton;
    private FlyoutViewWinUI.FlyoutBehavior? flyoutBehavior;

    public AgentWinUI(CreateAgentArgs args) : base(args)
    {
        _context = args.Context;
        
        var flyout = args.Behaviors.FirstOrDefault(x => x is FlyoutViewWinUI.FlyoutBehavior) as FlyoutViewWinUI.FlyoutBehavior;
        if (flyout != null)
            OnBehaiorAdded(flyout);

#if WINDOWS
        InputTransparent = true;
        CascadeInputTransparent = false;
#endif
    }

    public Rect[] UndragArea
    {
        get
        {
            var list = new List<Rect>();

            if (_flytoutButton != null && _flytoutButton.IsVisible)
                list.Add(_flytoutButton.Frame);

            foreach (var item in Children)
                if (item is IWindowsBehavior v)
                    list.AddRange(v.UndragArea);

            if (ViewWrapper.View is IWindowsBehavior vb)
                list.AddRange(vb.UndragArea);

            return list.ToArray();
        }
    }

    public override void OnBehaiorAdded(IBehavior newBehaior)
    {
        if (newBehaior is FlyoutViewWinUI.FlyoutBehavior b)
        {
            flyoutBehavior = b;
            flyoutBehavior.Changed += SetupFlyoutOffsets;

            if (_flytoutButton != null)
                _flytoutButton.IsVisible = true;
            SetupFlyoutOffsets(b);
        }
    }

    public override void OnBehaiorRemoved(IBehavior removedBehaior)
    {
        if (removedBehaior == flyoutBehavior)
        {
            if (_flytoutButton != null)
                _flytoutButton.IsVisible = false;
            flyoutBehavior.Changed -= SetupFlyoutOffsets;
            flyoutBehavior = null;
        }
    }

    public override Size ArrangeChildren(Rect bounds)
    {
        var res = base.ArrangeChildren(bounds);
        Scaffold.PlatformSpec.UpdateDesktopDragArea();
        return res;
    }

    public override Size Measure(double widthConstraint, double heightConstraint)
    {
        var res = base.Measure(widthConstraint, heightConstraint);
        
        if (_flytoutButton is IView button)
            button.Measure(widthConstraint, heightConstraint);

        return res;
    }

    public override void PrepareAnimate(NavigatingTypes type)
    {
        switch (type)
        {
            case NavigatingTypes.Push:
                Opacity = 0;
                TranslationX = 130;
                break;
            case NavigatingTypes.UnderPush:
                break;
            case NavigatingTypes.Pop:
                break;
            case NavigatingTypes.UnderPop:
                break;
            case NavigatingTypes.Replace:
                Opacity = 0;
                break;
            case NavigatingTypes.UnderReplace:
                break;
            default:
                break;
        }
    }

    public override AnimationInfo GetAnimation(NavigatingTypes animationType)
    {
        switch (animationType)
        {
            case NavigatingTypes.Push:
                return new AnimationInfo
                {
                    Easing = Easing.Linear,
                    Time = 140,
                };
            case NavigatingTypes.Pop:
                return new AnimationInfo
                {
                    Easing = Easing.Linear,
                    Time = 140,
                };
            case NavigatingTypes.Replace:
                return new AnimationInfo
                {
                    Easing = Easing.Linear,
                    Time = 140,
                };
            default:
                throw new NotSupportedException();
        }
    }

    public override void DoAnimation(double toFill, NavigatingTypes animType)
    {
        double toZero = 1 - toFill;
        switch (animType)
        {
            case NavigatingTypes.Push:
                Opacity = toFill;
                TranslationX *= toZero;
                break;
            case NavigatingTypes.UnderPush:
                break;
            case NavigatingTypes.Pop:
                Opacity = toZero;
                break;
            case NavigatingTypes.UnderPop:
                break;
            case NavigatingTypes.Replace:
                Opacity = toFill;
                break;
            case NavigatingTypes.UnderReplace:
                break;
            default:
                break;
        }
    }

    private void SetupFlyoutOffsets(FlyoutViewWinUI.FlyoutBehavior flyout)
    {
    }
}
