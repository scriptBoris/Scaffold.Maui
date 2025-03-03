using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.Common;

public class CommonAgent : Agent
{
    private readonly IScaffold _context;

    public virtual uint PushAnimationTime => 180;
    public virtual uint PopAnimationTime => 180;
    public virtual uint ReplaceAnimationTime => 180;

    public CommonAgent(CreateAgentArgs args) : base(args)
    {
        _context = args.Context;
        BindingContext = null;
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
                };
            case NavigatingTypes.Pop:
                return new AnimationInfo
                {
                    Easing = Easing.CubicOut,
                    Time = PopAnimationTime,
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
}
