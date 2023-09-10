using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ScaffoldLib.Maui.Internal
{
    internal class GlassHandler : ViewHandler<GlassView, UIVisualEffectViewMaui>
    {
        public GlassHandler() : base(Mapper)
        {
        }

        private static readonly PropertyMapper<GlassView, GlassHandler> Mapper = new()
        {
            [nameof(GlassView.Appearance)] = (h, v) =>
            {
                h.UpdateAppearance();
            },
            [nameof(GlassView.Content)] = (h, v) =>
            {
                h.UpdateContent();
            },
            [nameof(GlassView.CornerRadius)] = (h, v) =>
            {
                h.UpdateCornerRadius();
            }
        };

        private UIView? content;

        protected override UIVisualEffectViewMaui CreatePlatformView()
        {
            var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight);
            var blurView = new UIVisualEffectViewMaui(blurEffect);
            return blurView;
        }

        private void UpdateContent()
        {
            if (PlatformView == null)
                return;

            content?.RemoveFromSuperview();

            if (VirtualView.Content != null)
            {
                content = VirtualView.Content.ToPlatform(MauiContext!);
                PlatformView.ContentView.AddSubview(content);
            }

            PlatformView.SetNeedsLayout();
        }

        private void UpdateCornerRadius()
        {
            if (PlatformView == null)
                return;

            PlatformView.Layer.CornerRadius = (nfloat)VirtualView.CornerRadius;
            PlatformView.ClipsToBounds = true;
            PlatformView.SetNeedsLayout();
        }

        private void UpdateAppearance()
        {
            if (PlatformView == null)
                return;

            if (VirtualView.Appearance == AppTheme.Dark)
            {
                PlatformView.Effect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark);
            }
            else
            {
                PlatformView.Effect = UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight);
            }
        }
    }

    internal class UIVisualEffectViewMaui : UIVisualEffectView
    {
        public UIVisualEffectViewMaui() { }

        public UIVisualEffectViewMaui(UIVisualEffect effect) : base(effect) { }

        public override void SetNeedsDisplay()
        {
            base.SetNeedsDisplay();
            Superview?.SetNeedsLayout();
        }
    }
}
