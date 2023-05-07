using Microsoft.Maui.Layouts;
using Scaffold.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Containers
{
    public class ViewWrapper : Layout, ILayoutManager, IDisposable, IViewWrapper
    {
        private View? _overlay;

        public ViewWrapper(View view)
        {
            View = view;
            this.SetAppThemeColor(BackgroundColorProperty, Color.FromArgb("#eee"), Color.FromArgb("#343434"));

            view.BindingContextChanged += OnBindingContextChanged;
            Children.Add(view);

            if (view.BindingContext != null)
                SetupBindingContext();
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

        private void OnBindingContextChanged(object? sender, EventArgs e)
        {
            SetupBindingContext();
        }

        protected virtual void SetupBindingContext()
        {
            var menu = ScaffoldView.GetMenuItems(View);
            foreach (var item in menu)
                item.BindingContext = View.BindingContext;
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

        public void Dispose()
        {
            View.BindingContextChanged -= OnBindingContextChanged;
        }
    }
}
