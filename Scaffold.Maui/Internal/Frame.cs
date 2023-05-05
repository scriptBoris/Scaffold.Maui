using Microsoft.Maui.Layouts;
using Scaffold.Maui.Containers;
using Scaffold.Maui.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Internal
{
    [DebuggerDisplay($"View :: {{{nameof(ViewType)}}}")]
    internal class Frame : Layout, ILayoutManager, IFrame
    {
        public Frame(View view)
        {
            View = view;
            if (ScaffoldView.GetHasNavigationBar(view))
            {
                NavigationBar = CreateNavigationBar(view);
                if (NavigationBar != null)
                    Children.Add(NavigationBar);
            }

            ViewContainer = new(view);
            Children.Add(ViewContainer);
        }

        public INavigationBar? NavigationBar { get; private set; }
        public ViewWrapper ViewContainer { get; private set; }
        internal View View { get; private set; }
        internal string ViewType => View.GetType().Name;

        public Size ArrangeChildren(Rect bounds)
        {
            double offsetY = 0;
            if (NavigationBar is IView bar)
            {
                offsetY = bar.DesiredSize.Height;
                bar.Arrange(new Rect(0, 0, bounds.Width, bar.DesiredSize.Height));
            }

            if (ViewContainer is IView view)
            {
                double h = bounds.Height - offsetY;
                view.Arrange(new Rect(0, offsetY, bounds.Width, h));
            }

            return bounds.Size;
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            double freeH = heightConstraint;

            if (NavigationBar  is IView bar)
            {
                var m = bar.Measure(widthConstraint, freeH);
                freeH -= m.Height;
            }

            if (ViewContainer is IView view) 
            {
                view.Measure(widthConstraint, freeH);
            }

            return new Size(widthConstraint, heightConstraint);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return this;
        }

        internal async Task UpdateVisual(NavigatingArgs e)
        {
            var tasks = new List<Task>();
            tasks.Add(CommonAnimation(e));

            if (NavigationBar != null)
                tasks.Add(NavigationBar.UpdateVisual(e));

            tasks.Add(ViewContainer.UpdateVisual(e));
            await Task.WhenAll(tasks);
        }

        private async Task CommonAnimation(NavigatingArgs e)
        {
            if (!e.IsAnimating)
                return;

            if (e.NavigationType == NavigatingTypes.Replace)
            {
                this.Opacity = 0;
                await this.FadeTo(1, ScaffoldView.AnimationTime);
                return;
            }

            bool oldHasBar = e.OldContent != null ? ScaffoldView.GetHasNavigationBar(e.OldContent) : false;
            bool newHasBar = ScaffoldView.GetHasNavigationBar(e.NewContent);
            if (oldHasBar != newHasBar)
            {
                switch (e.NavigationType)
                {
                    case NavigatingTypes.Push:
                        this.Opacity = 0;
                        this.TranslationX = 100;
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

        private INavigationBar? CreateNavigationBar(View view)
        {
#if ANDROID
            return new Platforms.Android.NavigationBar(view);
#else
            return null;
#endif
        }

        internal void UpdateTitle(string? title)
        {
            if (NavigationBar != null)
            {
                NavigationBar.Title = title;
            }
        }

        internal void UpdateNavigationBarVisible(bool isVisible)
        {
            if (isVisible)
            {
                NavigationBar = CreateNavigationBar(View);
                if (NavigationBar != null)
                    Children.Add(NavigationBar);
            }
            else
            {
                Children.Remove(NavigationBar);
                NavigationBar = null;
            }
        }
    }
}
