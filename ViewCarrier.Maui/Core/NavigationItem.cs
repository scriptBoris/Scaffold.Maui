using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core
{
    public class NavigationItem : Layout, ILayoutManager, IDisposable
    {
        public NavigationItem(View view)
        {
            this.View = view;
            this.SetAppThemeColor(BackgroundColorProperty, Color.FromArgb("#eee"), Color.FromArgb("#343434"));

            view.BindingContextChanged += OnBindingContextChanged;
            Children.Add(view);

            if (view.BindingContext != null)
                SetupBindingContext();
        }

        public View View { get; private set; }

        protected override ILayoutManager CreateLayoutManager()
        {
            return this;
        }

        public virtual Size ArrangeChildren(Rect bounds)
        {
            return ((IView)View).Arrange(bounds);
        }

        public virtual Size Measure(double widthConstraint, double heightConstraint)
        {
            return ((IView)View).Measure(widthConstraint, heightConstraint);
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

        public void Dispose()
        {
            View.BindingContextChanged -= OnBindingContextChanged;
        }
    }
}
