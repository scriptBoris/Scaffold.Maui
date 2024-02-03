using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Args;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;


#if IOS
using UIKit;
using CoreGraphics;
#endif

namespace ScaffoldLib.Maui.Containers.Cupertino;

public class AgentCupertino : Agent
{
    private readonly IScaffold _context;

    public virtual uint PushAnimationTime => 180;
    public virtual uint PopAnimationTime => 180;
    public virtual uint ReplaceAnimationTime => 180;

    public AgentCupertino(CreateAgentArgs args) : base(args)
    {
        _context = args.Context;
        BindingContext = null;

        this.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                Scaffold.TryHideKeyboard();
            })
        });
    }

    public override void PrepareAnimate(NavigatingTypes type)
    {
        switch (type)
        {
            case NavigatingTypes.Replace:
                Opacity = 0;
                break;
            case NavigatingTypes.Push:
                TranslationX = ((View)_context).Width;
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
                    Easing = Easing.CubicOut,
                    Time = PushAnimationTime,
                    UsingPlatformAnimation = true,
                };
            case NavigatingTypes.Pop:
                return new AnimationInfo
                {
                    Easing = Easing.CubicOut,
                    Time = PopAnimationTime,
                    UsingPlatformAnimation = true,
                };
            case NavigatingTypes.Replace:
                return new AnimationInfo
                {
                    Easing = Easing.Linear,
                    Time = ReplaceAnimationTime,
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
                TranslationX *= toZero;
                break;
            case NavigatingTypes.UnderPush:
                TranslationX = -50 * toFill;
                break;
            case NavigatingTypes.Pop:
                TranslationX = Width * toFill;
                break;
            case NavigatingTypes.UnderPop:
                TranslationX *= toZero;
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

    public override async Task DoPlatformAnimation(NavigatingTypes animType)
    {
#if IOS
        switch (animType)
        {
            case NavigatingTypes.Push:
                {
                    var native = (UIView)this.Handler!.PlatformView!;
                    var tsc = new TaskCompletionSource();
                    native.BeginInvokeOnMainThread(() =>
                    {
                        var context = (View)_context;
                        var frame = new Rect(0, 0, context.Width, context.Height);

                        // offset
                        frame = frame.SetXY(frame.Width, 0);

                        double dur = (double)PushAnimationTime / 1000.0;
                        var animator = new UIViewPropertyAnimator(dur, UIViewAnimationCurve.EaseInOut,
                            () =>
                            {
                                native.Frame = frame.SetXY(0, 0);
                            });
                        animator.UserInteractionEnabled = false;
                        animator.AddCompletion(pos =>
                        {
                            this.TranslationX = 0;
                            tsc.TrySetResult();
                        });
                        animator.StartAnimation();
                    });
                    await tsc.Task;
                }
                break;
            case NavigatingTypes.UnderPush:
                {
                    var native = (UIView)this.Handler!.PlatformView!;
                    var tsc = new TaskCompletionSource();
                    native.BeginInvokeOnMainThread(() =>
                    {
                        var context = (View)_context;
                        var frame = new Rect(0, 0, context.Width, context.Height);

                        double dur = (double)PushAnimationTime / 1000.0;
                        var animator = new UIViewPropertyAnimator(dur, UIViewAnimationCurve.EaseInOut,
                            () =>
                            {
                                native.Frame = frame.SetXY(-200, 0);
                            });
                        animator.UserInteractionEnabled = false;
                        animator.AddCompletion(pos =>
                        {
                            this.TranslationX = -200;
                            tsc.TrySetResult();
                        });
                        animator.StartAnimation();
                    });
                    await tsc.Task;
                }
                break;
            case NavigatingTypes.Pop:
                break;
            case NavigatingTypes.UnderPop:
                break;
            case NavigatingTypes.Replace:
                break;
            case NavigatingTypes.UnderReplace:
                break;
            default:
                break;
        }
#endif
    }
}

#if IOS
public static class IOSExt
{
    public static CGRect OffsetBy(this CGRect self, nfloat x, nfloat y)
    {
        return new CGRect(self.X + x, self.Y + y, self.Width, self.Height);
    }

    public static CGRect OffsetBy(this CGRect self, double x, double y)
    {
        return new CGRect(self.X + x, self.Y + y, self.Width, self.Height);
    }

    public static CGRect SetXY(this CGRect self, double x, double y)
    {
        return new CGRect(x, y, self.Width, self.Height);
    }

    public static Rect SetXY(this Rect self, float x, float y)
    {
        return new Rect(x, y, self.Width, self.Height);
    }

    public static Rect SetXY(this Rect self, double x, double y)
    {
        return new Rect(x, y, self.Width, self.Height);
    }
}
#endif