using CoreGraphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using ObjCRuntime;

namespace ScaffoldLib.Maui.Internal
{
    internal class GlassHandler : ViewHandler<GlassView, UIVisualEffectViewMaui>
    {
        public GlassHandler() : base(Mapper)
        {
        }

        private static readonly PropertyMapper<GlassView, GlassHandler> Mapper = new(ViewMapper)
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

        protected override UIVisualEffectViewMaui CreatePlatformView()
        {
            var blurView = new UIVisualEffectViewMaui()
            {
                CrossPlatformLayout = VirtualView,
                Effect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark),
            };
            return blurView;
        }

        public override void SetVirtualView(IView view)
        {
            base.SetVirtualView(view);
            PlatformView.CrossPlatformLayout = VirtualView;
        }

        protected override void DisconnectHandler(UIVisualEffectViewMaui platformView)
        {
            base.DisconnectHandler(platformView);
            platformView.ContentView.ClearSubviews();
        }

        private void UpdateContent()
        {
            var handler = this;
            _ = handler.PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
            _ = handler.VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
            _ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

            var platformView = handler.PlatformView;
            platformView.ContentView.ClearSubviews();

            if (handler.VirtualView.PresentedContent is IView content)
            {
                var platformContent = content.ToPlatform(handler.MauiContext);
                platformView.ContentView.AddSubview(platformContent);
            }
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

    public class UIVisualEffectViewMaui : UIVisualEffectView
    {
        private static bool? _respondsToSafeArea;
        private double _lastMeasureHeight = double.NaN;
        private double _lastMeasureWidth = double.NaN;

        private WeakReference<IContentView>? _crossPlatformLayoutReference;

        public IContentView? CrossPlatformLayout
        {
            get => _crossPlatformLayoutReference != null && _crossPlatformLayoutReference.TryGetTarget(out var v) ? v : null;
            set => _crossPlatformLayoutReference = value == null ? null : new WeakReference<IContentView>(value);
        }

        private Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
        {
            return CrossPlatformLayout?.CrossPlatformMeasure(widthConstraint, heightConstraint) ?? Size.Zero;
        }

        private Size CrossPlatformArrange(Rect bounds)
        {
            return CrossPlatformLayout?.CrossPlatformArrange(bounds) ?? Size.Zero;
        }

        private bool RespondsToSafeArea()
        {
            if (_respondsToSafeArea.HasValue)
                return _respondsToSafeArea.Value;
            return (bool)(_respondsToSafeArea = RespondsToSelector(new Selector("safeAreaInsets")));
        }

        protected CGRect AdjustForSafeArea(CGRect bounds)
        {
            if (CrossPlatformLayout is not ISafeAreaView sav || sav.IgnoreSafeArea || !RespondsToSafeArea())
            {
                return bounds;
            }

#pragma warning disable CA1416 // TODO 'UIView.SafeAreaInsets' is only supported on: 'ios' 11.0 and later, 'maccatalyst' 11.0 and later, 'tvos' 11.0 and later.
            return SafeAreaInsets.InsetRect(bounds);
#pragma warning restore CA1416
        }

        protected bool IsMeasureValid(double widthConstraint, double heightConstraint)
        {
            // Check the last constraints this View was measured with; if they're the same,
            // then the current measure info is already correct and we don't need to repeat it
            return heightConstraint == _lastMeasureHeight && widthConstraint == _lastMeasureWidth;
        }

        protected void InvalidateConstraintsCache()
        {
            _lastMeasureWidth = double.NaN;
            _lastMeasureHeight = double.NaN;
        }

        protected void CacheMeasureConstraints(double widthConstraint, double heightConstraint)
        {
            _lastMeasureWidth = widthConstraint;
            _lastMeasureHeight = heightConstraint;
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            if (CrossPlatformLayout == null)
            {
                return base.SizeThatFits(size);
            }

            var widthConstraint = size.Width;
            var heightConstraint = size.Height;

            var crossPlatformSize = CrossPlatformMeasure(widthConstraint, heightConstraint);

            CacheMeasureConstraints(widthConstraint, heightConstraint);

            return crossPlatformSize.ToCGSize();
        }

        // TODO: Possibly reconcile this code with ViewHandlerExtensions.LayoutVirtualView
        // If you make changes here please review if those changes should also
        // apply to ViewHandlerExtensions.LayoutVirtualView
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (CrossPlatformLayout == null)
            {
                return;
            }

            var bounds = AdjustForSafeArea(Bounds).ToRectangle();

            var widthConstraint = bounds.Width;
            var heightConstraint = bounds.Height;

            // If the SuperView is a MauiView (backing a cross-platform ContentView or Layout), then measurement
            // has already happened via SizeThatFits and doesn't need to be repeated in LayoutSubviews. But we
            // _do_ need LayoutSubviews to make a measurement pass if the parent is something else (for example,
            // the window); there's no guarantee that SizeThatFits has been called in that case.
            bool IsMeasureInvalid = !IsMeasureValid(widthConstraint, heightConstraint);
            if (IsMeasureInvalid && Superview is not MauiView)
            {
                CrossPlatformMeasure(widthConstraint, heightConstraint);
                CacheMeasureConstraints(widthConstraint, heightConstraint);
            }

            CrossPlatformArrange(bounds);
        }

        public override void SetNeedsLayout()
        {
            InvalidateConstraintsCache();
            base.SetNeedsLayout();
            Superview?.SetNeedsLayout();
        }
    }
}
