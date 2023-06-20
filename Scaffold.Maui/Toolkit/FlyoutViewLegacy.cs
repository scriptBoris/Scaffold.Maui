using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit
{
    internal class FlyoutBackButtonBehavior : IBackButtonBehavior
    {
        private readonly FlyoutViewLegacy _parent;

        public FlyoutBackButtonBehavior(FlyoutViewLegacy parent)
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

        public View? LeftViewExtended(IScaffold context)
        {
#if WINDOWS
            var cmd = new Command(() =>
            {
                _parent.IsPresented = !_parent.IsPresented;
            });
            return new ButtonSam.Maui.Button
            {
                Margin = new Thickness(5,5,0,5),
                Padding = 6,
                BackgroundColor = Colors.Transparent,
                CornerRadius = 5,
                TapCommand = cmd,
                Content = new ImageTint
                {
                    HeightRequest = 18,
                    WidthRequest = 18,
                    Source = "ic_scaffold_menu.png",
                },
            };
#else
            return null;
#endif
        }
    }

    [ContentProperty(nameof(Flyout))]
    public class FlyoutViewLegacy : ZLayout, IScaffoldProvider, IAppear, IDisappear, IRemovedFromNavigation
    {
        private readonly ZLayout _panelDetail;
        private readonly ContentView _panelFlyout;
        private readonly View _panelFlyoutBackground;

        public FlyoutViewLegacy()
        {
            Scaffold.SetHasNavigationBar(this, false);
            VerticalOptions = LayoutOptions.Fill;
            HorizontalOptions = LayoutOptions.Fill;

            // detail
            _panelDetail = new()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
            };
            Children.Add(_panelDetail);

            // flyout background
            _panelFlyoutBackground = new BoxView
            {
                IsVisible = false,
                Opacity = 0,
                Color = Color.FromArgb("#6000"),
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
            };
            _panelFlyoutBackground.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => IsPresented = false),
            });
            Children.Add(_panelFlyoutBackground);

            // flyot
            _panelFlyout = new()
            {
                IsVisible = false,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Start,
            };
            _panelFlyout.SetAppTheme(ContentView.BackgroundColorProperty, Colors.White, Colors.Black);
            Children.Add(_panelFlyout);
        }

        #region bindable props
        // is presented
        public static readonly BindableProperty IsPresentedProperty = BindableProperty.Create(
            nameof(IsPresented),
            typeof(bool),
            typeof(FlyoutViewLegacy),
            false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, o, n) =>
            {
                if (b is FlyoutViewLegacy self)
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
            typeof(FlyoutViewLegacy),
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is FlyoutViewLegacy self)
                    self._panelFlyout.Content = (n as View);
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
            typeof(FlyoutViewLegacy),
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is FlyoutViewLegacy self)
                    self.UpdateDetail(n as View, o as View);
            }
        );
        public View? Detail
        {
            get => GetValue(DetailProperty) as View;
            set => SetValue(DetailProperty, value);
        }
        #endregion bindable props

        public IScaffold? ProvideScaffold => Detail as IScaffold;

        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    base.OnSizeAllocated(width, height);
        //    if (!IsPresented)
        //    {
        //        var def = new GridLength[]
        //        {
        //            new GridLength(3, GridUnitType.Star),
        //            GridLength.Star,
        //        };
        //        _panelFlyout.TranslationX = -CalculateWidthRules(def, width)[0];
        //    }
        //}

        private bool isFirst = true;
        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            var w = widthConstraint * 0.7;
            _panelFlyout.WidthRequest = w;

            //if (!IsPresented)
            if (isFirst && !IsPresented)
            {
                _panelFlyout.TranslationX = -w;
                isFirst = false;
            }

            return base.Measure(widthConstraint, heightConstraint);
        }

        private async void UpdateDetail(View? view, View? oldDetail)
        {
            if (view is Scaffold scaffold)
                scaffold.BackButtonBehavior ??= new FlyoutBackButtonBehavior(this);

            bool isAnimate = _panelDetail.Children.Count > 0;

            oldDetail?.TryDisappearing();

            if (view != null)
            {
                view.Opacity = isAnimate ? 0 : 1;
                _panelDetail.Children.Add(view);

                view.TryAppearing();

                if (isAnimate)
                {
                    await view.AwaitHandler();
                    await view.FadeTo(1, 180);
                }
                view.TryAppearing(true);
            }

            for (int i = _panelDetail.Children.Count - 1; i >= 0; i--)
            {
                var child = _panelDetail.Children[i];
                if (child == view)
                    continue;

                _panelDetail.Children.RemoveAt(i);
            }

            oldDetail?.TryDisappearing(true);
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
            _panelFlyout.IsVisible = true;
            _panelFlyoutBackground.IsVisible = true;
            _panelFlyout.TranslateTo(0, 0, 180, Easing.SinIn);
            _panelFlyoutBackground.FadeTo(1, 180);
        }

        private async void Hide()
        {
            await Task.WhenAll(
                _panelFlyout.TranslateTo(-_panelFlyout.Width, 0, 180, Easing.SinOut),
                _panelFlyoutBackground.FadeTo(0, 180)
            );

            _panelFlyoutBackground.IsVisible = false;
            _panelFlyout.IsVisible = false;
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

        public void OnAppear(bool isComplete)
        {
            Detail?.TryAppearing(isComplete);
        }

        public void OnDisappear(bool isComplete)
        {
            Detail?.TryDisappearing(isComplete);
        }

        public virtual void OnRemovedFromNavigation()
        {
            if (Detail is IRemovedFromNavigation rm)
                rm.OnRemovedFromNavigation();
        }
    }
}
