using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.WinUI
{
    public class WinUIFrame : Frame
    {
        //double offsetY = 20;

        public WinUIFrame(View view, ViewFactory viewFactory) : base(view, viewFactory)
        {
            //TranslationY = -offsetY;
        }

        //public override Size ArrangeChildren(Rect bounds)
        //{
        //    var b = new Rect(0,0, bounds.Width, bounds.Height + offsetY);
        //    return base.ArrangeChildren(b);
        //}

        //public override Size Measure(double widthConstraint, double heightConstraint)
        //{
        //    heightConstraint += offsetY;
        //    return base.Measure(widthConstraint, heightConstraint);
        //}
    }
}
