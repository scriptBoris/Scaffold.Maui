using ButtonSam.Maui.Core;
using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers
{
    public class DefaultAgent : Agent
    {
        private readonly IScaffold _context;

        public DefaultAgent(AgentArgs args) : base(args)
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

        public override Task Animate(NavigatingTypes type, CancellationToken cancellationToken)
        {
            switch (type)
            {
                case NavigatingTypes.Replace:
                    return this.FadeTo(1, Scaffold.AnimationTime);
                case NavigatingTypes.Push:
                    return this.TranslateTo(0, 0, Scaffold.AnimationTime, Easing.CubicOut);
                case NavigatingTypes.Pop:
                    return this.TranslateTo(Width, 0, Scaffold.AnimationTime, Easing.CubicOut);
                case NavigatingTypes.UnderPush:
                    return this.TranslateTo(-50, 0, Scaffold.AnimationTime, Easing.CubicOut);
                case NavigatingTypes.UnderPop:
                    return this.TranslateTo(0, 0, Scaffold.AnimationTime, Easing.CubicOut);
                default:
                    return Task.CompletedTask;
            }
        }

        public override void AnimationFunction(double toFill, NavigatingTypes animType)
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
}
