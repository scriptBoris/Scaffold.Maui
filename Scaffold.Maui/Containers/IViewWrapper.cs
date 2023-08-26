using Microsoft.Maui.Layouts;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers
{
    public interface IViewWrapper : IDisposable
    {
        View? Overlay { get; set; }
        View View { get; }

        void UpdateSafeArea(Thickness safeArea);
    }

    public class ViewWrapper : Layout, ILayoutManager, IViewWrapper
    {
        private View? _overlay;

        public ViewWrapper(View view)
        {
            View = view;
            this.SetAppThemeColor(BackgroundColorProperty, Color.FromArgb("#eee"), Color.FromArgb("#242424"));
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

        public virtual void UpdateSafeArea(Thickness safeArea)
        {
        }

        public virtual void Dispose()
        {
            if (View is IRemovedFromNavigation vrm)
                vrm.OnRemovedFromNavigation();

            if (View.BindingContext is IRemovedFromNavigation vmrm)
                vmrm.OnRemovedFromNavigation();

            Handler = null;
        }
    }
}
