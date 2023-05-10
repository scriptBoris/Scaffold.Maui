using Microsoft.Maui.Layouts;
using Scaffold.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Containers
{
    public class ViewWrapper : Layout, ILayoutManager, IViewWrapper
    {
        private View? _overlay;

        public ViewWrapper(View view)
        {
            View = view;
            this.SetAppThemeColor(BackgroundColorProperty, Color.FromArgb("#eee"), Color.FromArgb("#343434"));
            Children.Add(view);
        }

        public View View { get; private set; }
        public View? Overlay 
        {
            get => _overlay;
            set 
            {
                if (_overlay != null)
                {
                    Children.Remove(_overlay);
                }

                _overlay = value;

                if (_overlay != null)
                {
                    Children.Add(value);
                }
            } 
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return this;
        }

        public virtual Size ArrangeChildren(Rect bounds)
        {
            ((IView)View).Arrange(bounds);

            if (Overlay is IView overlay)
                overlay.Arrange(bounds);

            return bounds.Size;
        }

        public virtual Size Measure(double widthConstraint, double heightConstraint)
        {
            ((IView)View).Measure(widthConstraint, heightConstraint);

            if (Overlay is IView overlay)
                overlay.Measure(widthConstraint, heightConstraint);

            return new Size(widthConstraint, heightConstraint);
        }

        public async Task UpdateVisual(NavigatingArgs e)
        {
            if (!e.IsAnimating)
                return;

            switch (e.NavigationType)
            {
                case NavigatingTypes.Push:
                    Opacity = 0;
                    TranslationX = 100;
                    await Task.WhenAll(
                        this.FadeTo(1, ScaffoldView.AnimationTime),
                        this.TranslateTo(0, 0, ScaffoldView.AnimationTime, Easing.CubicOut)
                    );
                    break;
                case NavigatingTypes.Pop:
                    await Task.WhenAll(
                        this.FadeTo(0, ScaffoldView.AnimationTime, Easing.CubicOut),
                        this.TranslateTo(50, 0, ScaffoldView.AnimationTime, Easing.CubicOut)
                    );
                    break;
                default:
                    break;
            }
        }
    }
}
