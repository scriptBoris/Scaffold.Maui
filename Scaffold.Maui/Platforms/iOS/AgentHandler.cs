using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ScaffoldLib.Maui.Platforms.iOS
{
    internal class UIAgentView : LayoutView
    {
#if DEBUG
        private const float Multiple = 0.5f;
        private const float MultipleSmall = 0.2f;
#else
        private const float Multiple = 1.0f;
        private const float MultipleSmall = 0.5f;
#endif

        private int hardBig;
        private int hardSmall;

        internal event EventHandler<int>? ViewIsDrawed;

        internal int CalculateHard(int hardCount)
        {
            hardBig = (int)(hardCount * Multiple);
            hardSmall = (int)(hardCount * MultipleSmall);

            return hardBig;
        }

        public override void DrawLayer(CALayer layer, CGContext context)
        {
            base.DrawLayer(layer, context);
            ViewIsDrawed?.Invoke(this, hardBig);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ViewIsDrawed?.Invoke(this, hardSmall);
        }

        public override void SubviewAdded(UIView uiview)
        {
            base.SubviewAdded(uiview);
            ViewIsDrawed?.Invoke(this, hardSmall);
        }

        public override void WillRemoveSubview(UIView uiview)
        {
            base.WillRemoveSubview(uiview);
            ViewIsDrawed?.Invoke(this, hardSmall);
        }
    }

    public class AgentHandler : LayoutHandler
    {
        protected override LayoutView CreatePlatformView()
        {
            return new UIAgentView();
        }
    }
}
