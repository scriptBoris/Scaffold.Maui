using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal
{
    public class ZLayout : Layout, ILayoutManager
    {
        public ZLayout()
        {
        }

        public virtual Size ArrangeChildren(Rect bounds)
        {
            foreach (var item in Children)
            {
                bool visible = ((View)item).IsVisible;
                if (!visible)
                    continue;

                double x = 0;
                double y = 0;
                double h = bounds.Height;
                double w = bounds.Width;

                switch (item.VerticalLayoutAlignment)
                {
                    case Microsoft.Maui.Primitives.LayoutAlignment.Fill:
                        break;
                    case Microsoft.Maui.Primitives.LayoutAlignment.Start:
                        h = item.DesiredSize.Height;
                        break;
                    case Microsoft.Maui.Primitives.LayoutAlignment.Center:
                        break;
                    case Microsoft.Maui.Primitives.LayoutAlignment.End:
                        h = item.DesiredSize.Height;
                        y = bounds.Height - h;
                        break;
                    default:
                        break;
                }

                switch (item.HorizontalLayoutAlignment)
                {
                    case Microsoft.Maui.Primitives.LayoutAlignment.Fill:
                        break;
                    case Microsoft.Maui.Primitives.LayoutAlignment.Start:
                        w = item.DesiredSize.Width;
                        break;
                    case Microsoft.Maui.Primitives.LayoutAlignment.Center:
                        break;
                    case Microsoft.Maui.Primitives.LayoutAlignment.End:
                        w = item.DesiredSize.Width;
                        x = bounds.Width - w;
                        break;
                    default:
                        break;
                }

                var r = new Rect(x, y, w, h);
                item.Arrange(r);
            }
            return bounds.Size;
        }

        public virtual Size Measure(double widthConstraint, double heightConstraint)
        {
            double w = 0;
            double h = 0;
            foreach (var item in Children)
            {
                var view = (View)item;
                if (!view.IsVisible)
                    continue;

                var s = item.Measure(widthConstraint, heightConstraint);
                
                if (s.Width > w)
                    w = s.Width;

                if (s.Height > h)
                    h = s.Height;
            }
            return new Size(widthConstraint, heightConstraint);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return this;
        }
    }
}
