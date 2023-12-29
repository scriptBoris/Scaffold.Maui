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
    private readonly ButtonSam.Maui.Button _flytoutButton;
    private FlyoutViewWinUI.FlyoutBehavior? flyoutBehavior;

    public AgentWinUI(CreateAgentArgs args) : base(args)
    {
        _context = args.Context;
        _flytoutButton = new ButtonSam.Maui.Button
        {
            Margin = 5,
            Padding = 6,
            BackgroundColor = Colors.Transparent,
            CornerRadius = 5,
            HorizontalOptions = LayoutOptions.Start,
            Content = new ImageTint
            {
                HeightRequest = 18,
                WidthRequest = 18,
                Source = "ic_scaffold_menu.png",
            },
            IsVisible = false,
            TapCommand = new Command(() =>
            {
                if (flyoutBehavior != null)
                    flyoutBehavior.InvokeChangePresented();
            }),
        };
        Children.Insert(2, _flytoutButton);

        var flyout = args.Behaviors.FirstOrDefault(x => x is FlyoutViewWinUI.FlyoutBehavior) as FlyoutViewWinUI.FlyoutBehavior;
        if (flyout != null)
            OnBehaiorAdded(flyout);
    }

    public Rect[] UndragArea
    {
        get
        {
            var list = new List<Rect>();

            if (_flytoutButton.IsVisible)
                list.Add(_flytoutButton.Frame);

            foreach (var item in Children)
                if (item is IWindowsBehavior v)
                    list.AddRange(v.UndragArea);

            return list.ToArray();
        }
    }

    public override void OnBehaiorAdded(IBehavior newBehaior)
    {
        if (newBehaior is FlyoutViewWinUI.FlyoutBehavior b)
        {
            flyoutBehavior = b;
            flyoutBehavior.Changed += SetupFlyoutOffsets;
            _flytoutButton.IsVisible = true;
            SetupFlyoutOffsets(b);
        }
    }

    public override void OnBehaiorRemoved(IBehavior removedBehaior)
    {
        if (removedBehaior == flyoutBehavior)
        {
            _flytoutButton.IsVisible = false;
            flyoutBehavior.Changed -= SetupFlyoutOffsets;
            flyoutBehavior = null;
        }
    }

    public override Size ArrangeChildren(Rect bounds)
    {
        double offsetY = 0;

        if (NavigationBar is IView bar)
        {
            offsetY = bar.DesiredSize.Height;
            bar.Arrange(new Rect(0, 0, bounds.Width, bar.DesiredSize.Height));
        }

        if (_flytoutButton is IView flyoutButton)
        {
            flyoutButton.Arrange(new Rect(0, 0, 
                _flytoutButton.DesiredSize.Width,
                _flytoutButton.DesiredSize.Height));
        }

        if (ViewWrapper is IView view)
        {
            double h = bounds.Height - offsetY;
            view.Arrange(new Rect(0, offsetY, bounds.Width, h));
        }

        if (ZBuffer is IView zbuffer)
        {
            zbuffer.Arrange(bounds);
        }

        Scaffold.PlatformSpec.UpdateDesktopDragArea();

        return bounds.Size;
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
        if (NavigationBar is Layout nav)
            nav.Padding = new Thickness(
                flyout.NavigationBarOffset,
                nav.Padding.Top,
                nav.Padding.Right,
                nav.Padding.Bottom
            );

        if (ViewWrapper is Layout wrapper)
            wrapper.Margin = new Thickness(flyout.ViewOffset, 0, 0, 0);
    }
}
