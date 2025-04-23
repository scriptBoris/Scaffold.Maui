using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

internal class SpecialAlertLayout : Layout, ILayoutManager
{
    private const double Separator_Height = 1;

    public SpecialAlertLayout()
    {
        GestureRecognizers.Add(new TapGestureRecognizer());
    }

    #region bindable props
    // title-body spacing
    public static readonly BindableProperty TitleBodySpacingProperty = BindableProperty.Create(
        nameof(TitleBodySpacing),
        typeof(double),
        typeof(SpecialAlertLayout),
        0.0,
        propertyChanged: (b, o, n) =>
        {
            if (b is SpecialAlertLayout self)
            {
                self.InvalidateMeasure();
            }
        }
    );
    public double TitleBodySpacing
    {
        get => (double)GetValue(TitleBodySpacingProperty);
        set => SetValue(TitleBodySpacingProperty, value);
    }

    // title-body margin
    public static readonly BindableProperty TitleBodyMarginProperty = BindableProperty.Create(
        nameof(TitleBodyMargin),
        typeof(Thickness),
        typeof(SpecialAlertLayout),
        new Thickness(0),
        propertyChanged: (b, o, n) =>
        {
            if (b is SpecialAlertLayout self)
            {
                self.InvalidateMeasure();
            }
        }
    );
    public Thickness TitleBodyMargin
    {
        get => (Thickness)GetValue(TitleBodyMarginProperty);
        set => SetValue(TitleBodyMarginProperty, value);
    }

    // title view
    public static readonly BindableProperty TitleViewProperty = BindableProperty.Create(
        nameof(TitleView),
        typeof(View),
        typeof(SpecialAlertLayout),
        null,
        propertyChanged: (b,o,n) =>
        {
            if (b is SpecialAlertLayout self)
            {
                if (o is View old)
                    self.Children.Remove(old);

                if (n is View nev)
                    self.Children.Add(nev);
                //self.InvalidateMeasure();
            }
        }
    );
    public View? TitleView
    {
        get => GetValue(TitleViewProperty) as View;
        set => SetValue(TitleViewProperty, value);
    }

    // body view
    public static readonly BindableProperty BodyViewProperty = BindableProperty.Create(
        nameof(BodyView),
        typeof(View),
        typeof(SpecialAlertLayout),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is SpecialAlertLayout self)
            {
                if (o is View old)
                    self.Children.Remove(old);

                if (n is View nev)
                    self.Children.Add(nev);
                //self.InvalidateMeasure();
            }
        }
    );
    public View? BodyView
    {
        get => GetValue(BodyViewProperty) as View;
        set => SetValue(BodyViewProperty, value);
    }

    // separator view
    public static readonly BindableProperty SeparatorViewProperty = BindableProperty.Create(
        nameof(SeparatorView),
        typeof(View),
        typeof(SpecialAlertLayout),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is SpecialAlertLayout self)
            {
                if (o is View old)
                    self.Children.Remove(old);

                if (n is View nev)
                    self.Children.Add(nev);
                //self.InvalidateMeasure();
            }
        }
    );
    public View? SeparatorView
    {
        get => GetValue(SeparatorViewProperty) as View;
        set => SetValue(SeparatorViewProperty, value);
    }

    // footer view
    public static readonly BindableProperty FooterViewProperty = BindableProperty.Create(
        nameof(FooterView),
        typeof(View),
        typeof(SpecialAlertLayout),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is SpecialAlertLayout self)
            {
                if (o is View old)
                    self.Children.Remove(old);

                if (n is View nev)
                    self.Children.Add(nev);
                //self.InvalidateMeasure();
            }
        }
    );
    public View? FooterView
    {
        get => GetValue(FooterViewProperty) as View;
        set => SetValue(FooterViewProperty, value);
    }
    #endregion bindable props

    public int BodyLength { get; set; }

    public Size ArrangeChildren(Rect bounds)
    {
        double x = TitleBodyMargin.Left;
        double y = TitleBodyMargin.Top;
        double titleBodyWidth = bounds.Width - TitleBodyMargin.HorizontalThickness;

        if (TitleView is IView title)
        {
            double h = title.DesiredSize.Height;
            var rect = new Rect(x, y, titleBodyWidth, h);
            title.Arrange(rect);
            y += h;
        }

        if (TitleView != null && BodyView != null)
            y += TitleBodySpacing;

        if (BodyView is IView body)
        {
            double h = body.DesiredSize.Height;
            double w = body.DesiredSize.Width;
            var rect = new Rect(x, y, w, h);
            body.Arrange(rect);
            y += h;
        }

        y += TitleBodyMargin.Bottom;

        if (SeparatorView is IView separator)
        {
            double h = separator.DesiredSize.Height;
            var rect = new Rect(0, y, bounds.Width, h);
            separator.Arrange(rect);
            y += h;
        }

        if (FooterView is IView footer)
        {
            double h = footer.DesiredSize.Height;
            var rect = new Rect(0, y, bounds.Width, h);
            footer.Arrange(rect);
            y += h;
        }

        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
#if WINDOWS
        double ww = 300;
        if (widthConstraint > 600 && BodyLength > 200)
            ww = 500;
#else
        double ww;

        if (widthConstraint < 250)
            ww = widthConstraint * 0.8;
        else
            ww = 250;

        if (widthConstraint > 270 && BodyLength > 200)
            ww = 270;
#endif
        double hh = Math.Max(heightConstraint - 50, 0);

        if (heightConstraint > 700)
            hh = 600;

        double freeH = hh - TitleBodyMargin.VerticalThickness;
        double height = 0 + TitleBodyMargin.VerticalThickness;
        double topBodyWidth = ww - TitleBodyMargin.HorizontalThickness;

        if (TitleView != null && BodyView != null)
        {
            freeH -= TitleBodySpacing;
            height += TitleBodySpacing;
        }

        if (TitleView is IView title)
        {
            var size = title.Measure(topBodyWidth, freeH);
            height += size.Height;
            freeH -= size.Height;
        }

        if (SeparatorView is IView separator)
        {
            separator.Measure(ww, Separator_Height);
            height += Separator_Height;
            freeH -= Separator_Height;
        }

        if (FooterView is IView footer)
        {
            var size = footer.Measure(ww, freeH);
            height += size.Height;
            freeH -= size.Height;
        }

        if (BodyView is IView body)
        {
            var size = body.Measure(topBodyWidth, freeH);
            height += size.Height;
            freeH -= size.Height;
        }

        return new Size(ww, height);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }
}