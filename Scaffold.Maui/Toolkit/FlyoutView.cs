using Scaffold.Maui.Core;
using Scaffold.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Toolkit
{
    internal class FlyoutBackButtonBehavior : IBackButtonBehavior
    {
        private readonly FlyoutView _parent;

        public FlyoutBackButtonBehavior(FlyoutView parent)
        {
            _parent = parent;
        }

        public ImageSource? OverrideBackButtonIcon(IScaffold context)
        {
            if (context.NavigationStack.Count <= 1)
                return ImageSource.FromFile("ic_scaffold_menu.png");

            return null;
        }

        public bool? OverrideBackButtonVisibility(IScaffold context)
        {
            return true;
        }

        public bool? OverrideSoftwareBackButtonAction(IScaffold context)
        {
            if (context.NavigationStack.Count <= 1)
            {
                _parent.IsPresented = !_parent.IsPresented;
                return true;
            }
            
            return null;
        }

        public bool? OverrideHardwareBackButtonAction(IScaffold context)
        {
            if (context.NavigationStack.Count <= 1 && _parent.IsPresented)
            {
                _parent.IsPresented = false;
                return true;
            }

            return null;
        }
    }

    [ContentProperty(nameof(Flyout))]
    public class FlyoutView : Grid, IScaffoldProvider
    {
        private readonly ContentView _flyoutPanel;
        private readonly Grid _detailPanel;
        private readonly View _darkPanel;

        public FlyoutView()
        {
            ScaffoldView.SetHasNavigationBar(this, false);
            ColumnSpacing = 0;
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Star),
            };

            // detail
            _detailPanel = new Grid();
            Grid.SetColumnSpan(_detailPanel, 2);
            Children.Add(_detailPanel);

            // dark
            _darkPanel = new BoxView
            {
                IsVisible = false,
                Opacity = 0,
                Color = Color.FromArgb("#6000"),
            };
            _darkPanel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => IsPresented = false),
            });
            Grid.SetColumnSpan(_darkPanel, 2);
            Children.Add(_darkPanel);

            // flyot
            _flyoutPanel = new()
            {
                IsVisible = false,
            };
            _flyoutPanel.SetAppTheme(ContentView.BackgroundColorProperty, Colors.White, Colors.Black);
            Grid.SetColumn(_flyoutPanel, 0);
            Children.Add(_flyoutPanel);
        }

        #region bindable props
        // is presented
        public static readonly BindableProperty IsPresentedProperty = BindableProperty.Create(
            nameof(IsPresented),
            typeof(bool),
            typeof(FlyoutView),
            false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, o, n) =>
            {
                if (b is FlyoutView self)
                    self.UpdateFlyoutMenuVisibility((bool)n);
            }
        );
        public bool IsPresented
        {
            get => (bool)GetValue(IsPresentedProperty);
            set => SetValue(IsPresentedProperty, value);
        }

        // flyout
        public static readonly BindableProperty FlyoutProperty = BindableProperty.Create(
            nameof(Flyout),
            typeof(View),
            typeof(FlyoutView),
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is FlyoutView self)
                    self._flyoutPanel.Content = (n as View);
            }
        );
        public View? Flyout
        {

            get => GetValue(FlyoutProperty) as View;
            set => SetValue(FlyoutProperty, value);
        }

        // detail
        public static readonly BindableProperty DetailProperty = BindableProperty.Create(
            nameof(Detail),
            typeof(View),
            typeof(FlyoutView),
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is FlyoutView self)
                    self.UpdateDetail(n as View);
            }
        );
        public View? Detail
        {
            get => GetValue(DetailProperty) as View;
            set => SetValue(DetailProperty, value);
        }
        #endregion bindable props

        public IScaffold? ProvideScaffold => Detail as IScaffold;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (!IsPresented)
            {
                var def = new GridLength[]
                {
                    new GridLength(3, GridUnitType.Star),
                    GridLength.Star,
                };
                _flyoutPanel.TranslationX = -CalculateWidthRules(def, width)[0];
            }
        }

        private async void UpdateDetail(View? view)
        {
            if (view is ScaffoldView scaffold)
                scaffold.BackButtonBehavior ??= new FlyoutBackButtonBehavior(this);

            bool isAnimate = _detailPanel.Children.Count > 0;

            if (view != null)
            {
                view.Opacity = isAnimate ? 0 : 1;
                _detailPanel.Children.Add(view);
                if (isAnimate)
                {
                    await view.AwaitHandler();
                    await view.FadeTo(1, 180);
                }
            }

            for (int i = _detailPanel.Children.Count - 1; i >= 0; i--)
            {
                var child = _detailPanel.Children[i];
                if (child == view)
                    continue;

                _detailPanel.Children.RemoveAt(i);
            }
        }

        private void UpdateFlyoutMenuVisibility(bool value)
        {
            if (value)
                Show();
            else
                Hide();
        }

        private void Show()
        {
            _flyoutPanel.IsVisible = true;
            _darkPanel.IsVisible = true;
            _flyoutPanel.TranslateTo(0, 0, 180, Easing.SinIn);
            _darkPanel.FadeTo(1, 180);
        }

        private async void Hide()
        {
            await Task.WhenAll(
                _flyoutPanel.TranslateTo(-_flyoutPanel.Width, 0, 180, Easing.SinOut),
                _darkPanel.FadeTo(0, 180)
            );

            _darkPanel.IsVisible = false;
            _flyoutPanel.IsVisible = false;
        }

        internal static double[] CalculateWidthRules(GridLength[] viewRules, double availableWidth)
        {
            double[] result = new double[viewRules.Length];
            double totalSizeStar = 0;
            double totalSizePixel = 0;
            double freeSpace = availableWidth;

            // Сначала проходим по всем элементам и считаем общую сумму значений GridLength в Star и Pixel
            for (int i = 0; i < viewRules.Length; i++)
            {
                if (viewRules[i].IsStar)
                {
                    totalSizeStar += viewRules[i].Value;
                }
                else if (viewRules[i].IsAbsolute)
                {
                    totalSizePixel += viewRules[i].Value;
                }
            }

            freeSpace -= totalSizePixel;
            if (freeSpace < 0)
                freeSpace = 0;

            // Затем проходим по всем элементам и вычисляем их фактические размеры
            for (int i = 0; i < viewRules.Length; i++)
            {
                double pixelSize = 0;
                double starSize = 0;

                if (viewRules[i].IsStar)
                {
                    if (viewRules[i].Value > 0 && totalSizeStar > 0)
                        starSize = freeSpace * (viewRules[i].Value / totalSizeStar);
                }
                else if (viewRules[i].IsAbsolute)
                {
                    pixelSize = viewRules[i].Value;
                }

                result[i] = pixelSize + starSize;
            }

            return result;
        }
    }
}
