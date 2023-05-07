using Scaffold.Maui.Core;
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

        public bool? OverrideBackButtonAction(IScaffold context)
        {
            if (context.NavigationStack.Count <= 1)
            {
                _parent.IsPresented = !_parent.IsPresented;
                return true;
            }

            return null;
        }
    }

    [ContentProperty(nameof(Flyout))]
    public class FlyoutView : Grid, IScaffoldProvider
    {
        private readonly ContentView _flyoutPanel;
        private readonly ContentView _detailPanel;
        private readonly View _darkPanel;

        public FlyoutView()
        {
            ColumnSpacing = 0;
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Star),
            };

            // detail
            _detailPanel = new ContentView();
            Grid.SetColumnSpan(_detailPanel, 2);
            Children.Add(_detailPanel);

            // dark
            _darkPanel = new BoxView
            {
                IsVisible = false,
                Color = Color.FromArgb("#6000"),
            };
            _darkPanel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => IsPresented = !IsPresented),
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
                //self._flyoutPanel.SetContent(n as View);
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

        private void UpdateDetail(View? view)
        {
            if (view is ScaffoldView scaffold)
                scaffold.BackButtonBehavior ??= new FlyoutBackButtonBehavior(this);

            _detailPanel.Content = view;
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
        }

        private void Hide()
        {
            _darkPanel.IsVisible = false;
            _flyoutPanel.IsVisible = false;
        }
    }
}
