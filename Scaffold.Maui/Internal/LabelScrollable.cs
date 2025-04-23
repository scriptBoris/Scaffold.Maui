using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

internal class LabelScrollable : ScrollView
{
    private readonly LabelNative _label;

    public LabelScrollable()
    {
        _label = new LabelNative
        {
            TextColor = (Color)TextColorProperty.DefaultValue
        };
        Orientation = ScrollOrientation.Vertical;
        Content = _label;
    }

    #region bindable props
    // style attribute
    public static readonly BindableProperty StyleAttributeProperty = BindableProperty.Create(
        nameof(StyleAttribute), 
        typeof(LabelNativeAttributes), 
        typeof(LabelScrollable),
        LabelNativeAttributes.None,
        propertyChanged: (b,o,n) =>
        {
            if (b is LabelScrollable self)
                self._label.StyleAttribute = (LabelNativeAttributes)n;
        }
    );
    public LabelNativeAttributes StyleAttribute
    {
        get => (LabelNativeAttributes)GetValue(StyleAttributeProperty);
        set => SetValue(StyleAttributeProperty, value);
    }

    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(LabelScrollable),
        "",
        propertyChanged: (b, o, n) =>
        {
            if (b is LabelScrollable self)
                self._label.Text = n as string;
        }
    );
    public string Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    // text color
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(LabelScrollable),
        Colors.Black,
        propertyChanged: (b, o, n) =>
        {
            if (b is LabelScrollable self)
                self._label.TextColor = n as Color;
        }
    );
    public Color TextColor
    {
        get => GetValue(TextColorProperty) as Color;
        set => SetValue(TextColorProperty, value);
    }
    #endregion bindable props
}