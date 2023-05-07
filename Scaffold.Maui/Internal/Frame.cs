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
    [DebuggerDisplay($"Frame :: {{{nameof(ViewType)}}}")]
    internal class Frame : Layout, ILayoutManager, IFrame
    {
        private readonly ViewFactory _viewFactory;
        private readonly View _view;
        private View? _overlay;

        public Frame(View view, ViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
            _view = view;

            DrawLayout();

            ViewWrapper = viewFactory.CreateViewWrapper(view);
            Children.Add((View)ViewWrapper);
        }

        internal string ViewType => _view.GetType().Name;
        public INavigationBar? NavigationBar { get; private set; }
        public IViewWrapper ViewWrapper { get; private set; }
        public View? Overlay 
        {
            get => _overlay;
            set
            {
                if (_overlay != null)
                {
                    _overlay.HandlerChanged -= Value_HandlerChanged;
                    Children.Remove(_overlay);
                }

                _overlay = value;

                if (_overlay != null)
                {
                    _overlay.HandlerChanged += Value_HandlerChanged;
                    Children.Add(value);
                }
            }
        }

        private void Value_HandlerChanged(object? sender, EventArgs e)
        {
#if ANDROID
            if (_overlay?.Handler?.PlatformView is Android.Views.View aview)
                aview.Elevation = 5;
#endif
        }

        public Size ArrangeChildren(Rect bounds)
        {
            double offsetY = 0;
            if (NavigationBar is IView bar)
            {
                offsetY = bar.DesiredSize.Height;
                bar.Arrange(new Rect(0, 0, bounds.Width, bar.DesiredSize.Height));
            }

            if (ViewWrapper is IView view)
            {
                double h = bounds.Height - offsetY;
                view.Arrange(new Rect(0, offsetY, bounds.Width, h));
            }

            if (Overlay is IView overlay)
            {
                overlay.Arrange(bounds);
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

            if (ViewWrapper is IView view) 
            {
                view.Measure(widthConstraint, freeH);
            }

            if (Overlay is IView overlay)
            {
                overlay.Measure(widthConstraint, heightConstraint);
            }

            return new Size(widthConstraint, heightConstraint);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return this;
        }

        public void DrawLayout()
        {
            bool oldIsVisible = NavigationBar != null;
            bool isVisible = ScaffoldView.GetHasNavigationBar(_view);
            if (oldIsVisible != isVisible)
            {
                if (isVisible)
                {
                    NavigationBar = _viewFactory.CreateNavigationBar(_view);
                    if (NavigationBar != null)
                        Children.Add((View)NavigationBar);
                }
                else
                {
                    Children.Remove(NavigationBar as View);
                    NavigationBar = null;
                }
            }
        }

        public async Task UpdateVisual(NavigatingArgs e)
        {
            var tasks = new List<Task> { CommonAnimation(e) };

            if (NavigationBar != null)
                tasks.Add(NavigationBar.UpdateVisual(e));

            tasks.Add(ViewWrapper.UpdateVisual(e));
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
    }
}
