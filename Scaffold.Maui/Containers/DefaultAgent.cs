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

        public DefaultAgent(AgentArgs args, IScaffold context) : base(args, context)
        {
            this._context = context;
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
    }
}
