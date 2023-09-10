using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

[ContentProperty(nameof(Content))]
internal class GlassView : View, IVisualTreeElement, IPadding
{
    #region bindable props
    // appearance 
    public static readonly BindableProperty AppearanceProperty = BindableProperty.Create(
        nameof(Appearance),
        typeof(AppTheme),
        typeof(GlassView),
        AppTheme.Light
    );
    public AppTheme Appearance
    {
        get => (AppTheme)GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    // content
    public static readonly BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(View),
        typeof(GlassView),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is GlassView self)
            {
                if (o is View old)
                {
                    old.Parent = null;
                    old.MeasureInvalidated -= self.ContentMeasureInvalidated;
                    old.SizeChanged -= self.Newest_SizeChanged;
                    self.OnChildRemoved(old, 0);
                }

                if (n is View newest)
                {
                    newest.Parent = self;
                    newest.MeasureInvalidated += self.ContentMeasureInvalidated;
                    newest.SizeChanged += self.Newest_SizeChanged;
                    self.OnChildAdded(newest);
                }
            }
        }
    );
    public View? Content
    {
        get => GetValue(ContentProperty) as View;
        set => SetValue(ContentProperty, value);
    }

    // padding
    public static readonly BindableProperty PaddingProperty = BindableProperty.Create(
        nameof(Padding),
        typeof(Thickness),
        typeof(GlassView),
        new Thickness(0),
        propertyChanged: (b, o, n) =>
        {
            if (b is GlassView self)
                self.InvalidateMeasure();
        }
    );
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    // corner radius
    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(GlassView),
        0.0
    );
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
    #endregion bindable props

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        double w = widthConstraint;
        double h = heightConstraint;

        if (WidthRequest >= 0)
            w = WidthRequest;

        if (HeightRequest >= 0)
            h = HeightRequest;

        if (MinimumWidthRequest >= 0 && w > MinimumWidthRequest)
            w = MinimumWidthRequest;

        if (MinimumHeightRequest >= 0 && h > MinimumHeightRequest)
            h = MinimumHeightRequest;

        var size = new Size(w, h);
        if (Content is IView content)
        {
            size = content.Measure(
                w - Padding.HorizontalThickness,
                h - Padding.VerticalThickness
            );

            size = size.WithPadding(Padding);
        }

        if (WidthRequest >= 0)
            size = new(WidthRequest, size.Height);

        if (HeightRequest >= 0)
            size = new(size.Width, HeightRequest);

        if (MinimumWidthRequest >= 0 && size.Width > MinimumWidthRequest)
            size = new Size(MinimumWidthRequest, size.Height);

        if (MinimumHeightRequest >= 0 && size.Height > MinimumHeightRequest)
            size = new Size(size.Width, MinimumHeightRequest);

        DesiredSize = size;
        return size;
    }

    protected override Size ArrangeOverride(Rect bounds)
    {
        var baseSize = base.ArrangeOverride(bounds);

        double x = bounds.X;
        double y = bounds.Y;
        double w = baseSize.Width;
        double h = baseSize.Height;

        if (Content is IView content)
        {
            var contentBounds = new Rect(
                x + Padding.Left,
                y + Padding.Top,
                w - Padding.HorizontalThickness,
                h - Padding.VerticalThickness
            );

            content.Arrange(contentBounds);
        }

        return baseSize;
    }

    protected override void InvalidateMeasureOverride()
    {
        base.InvalidateMeasureOverride();
    }

    IReadOnlyList<IVisualTreeElement> IVisualTreeElement.GetVisualChildren()
    {
        var list = new List<IVisualTreeElement>();
        if (Content != null)
            list.Add(Content);

        return list;
    }

    private void ContentMeasureInvalidated(object? sender, EventArgs e)
    {
        //InvalidateMeasure();
    }

    private void Newest_SizeChanged(object? sender, EventArgs e)
    {
        //InvalidateMeasure();
    }
}
