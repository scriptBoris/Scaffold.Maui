using Microsoft.Maui.Layouts;

namespace ScaffoldLib.Maui.Toolkit;

// todo зачем этот контроллер нужен?
[ContentProperty("Content")]
public class ContentView : Layout, ILayoutManager
{
    public static readonly BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(View),
        typeof(ContentView),
        null,
        propertyChanged:(b,o,n) =>
        {
            if (b is ContentView self)
            {
                if (o is View old)
                    self.Children.Remove(old);

                if (n is View nn)
                    self.Children.Add(nn);
            }
        }
    );
    public View? Content
    {
        get => GetValue(ContentProperty) as View;
        set => SetValue(ContentProperty, value);
    }

    public Size ArrangeChildren(Rect bounds)
    {
        if (Content is IView v)
        {
            double x = 0;
            double y = 0;
            double width;
            double height;

            bool isFillH = Content.HorizontalOptions.Alignment == LayoutAlignment.Fill || Content.HorizontalOptions.Expands;
            bool isFillV = Content.VerticalOptions.Alignment == LayoutAlignment.Fill || Content.VerticalOptions.Expands;
            if (isFillH)
            {
                width = bounds.Width;
            }
            else
            {
                width = Content.DesiredSize.Width;
            }

            if (isFillV)
            {
                height = bounds.Height;
            }
            else 
            {
                height = Content.DesiredSize.Height;
            }

            v.Arrange(new Rect(x, y, width, height));
        }
        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        double w = 0;
        double h = 0;
        bool isFill = this.HorizontalOptions.Alignment == LayoutAlignment.Fill || this.HorizontalOptions.Expands;

        Size contentSize;
        if (Content is IView v)
        {
            contentSize = v.Measure(widthConstraint, heightConstraint);
        }
        else
        {
            contentSize = default;
        }

        if (isFill)
        {
            w = widthConstraint;
            h = heightConstraint;
        }
        else
        {
            w = contentSize.Width;
            h = contentSize.Height;
        }

        return new Size(w, h);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }
}
