using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

[ContentProperty(nameof(Content))]
public class GlassView : View, IContentView, IVisualTreeElement
{
    private readonly List<IVisualTreeElement> _children = new();

    #region bindable props
    // appearance 
    public static readonly BindableProperty AppearanceProperty = BindableProperty.Create(
        nameof(Appearance),
        typeof(AppTheme),
        typeof(GlassView),
        AppTheme.Dark
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
                if (o is Element old)
                {
                    self._children.Remove(old);
                    self.OnChildRemoved(old, 0);
                }

                if (n is Element newest)
                {
                    self._children.Add(newest);
                    self.OnChildAdded(newest);
                }

                self.InvalidateMeasureHardcore();
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
                self.InvalidateMeasureHardcore();
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

    object? IContentView.Content => Content;
    public IView? PresentedContent => Content;

    public Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
    {
        return this.MeasureContent(widthConstraint, heightConstraint);
    }

    public Size CrossPlatformArrange(Rect bounds)
    {
        this.ArrangeContent(bounds);
        return bounds.Size;
    }

    IReadOnlyList<IVisualTreeElement> IVisualTreeElement.GetVisualChildren()
    {
        return _children;
    }
}
