using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

public class LabelNative : Label
{
    // attribute
    public static readonly BindableProperty StyleAttributeProperty = BindableProperty.Create(
        nameof(StyleAttribute),
        typeof(LabelNativeAttributes),
        typeof(LabelNative),
        LabelNativeAttributes.None
    );
    public LabelNativeAttributes StyleAttribute
    {
        get => (LabelNativeAttributes)GetValue(StyleAttributeProperty);
        set => SetValue(StyleAttributeProperty, value);
    }
}

//public class LabelNative : View, ITextAlignment
//{
//    // attribute
//    public static readonly BindableProperty StyleAttributeProperty = BindableProperty.Create(
//        nameof(StyleAttribute),
//        typeof(LabelNativeAttributes), 
//        typeof(LabelNative),
//        LabelNativeAttributes.None
//    );
//    public LabelNativeAttributes StyleAttribute
//    {
//        get => (LabelNativeAttributes)GetValue(StyleAttributeProperty);
//        set => SetValue(StyleAttributeProperty, value);
//    }

//    // text
//    public static readonly BindableProperty TextProperty = BindableProperty.Create(
//        nameof(Text),
//        typeof(string),
//        typeof(LabelNative),
//        null
//    );
//    public string? Text
//    {
//        get => GetValue(TextProperty) as string;
//        set => SetValue(TextProperty, value);
//    }

//    // text color
//    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
//        nameof(TextColor),
//        typeof(Color),
//        typeof(LabelNative),
//        null
//    );
//    public Color? TextColor
//    {
//        get => GetValue(TextColorProperty) as Color;
//        set => SetValue(TextColorProperty, value);
//    }

//    // font size
//    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
//        nameof(FontSize),
//        typeof(double),
//        typeof(LabelNative),
//        14.0
//    );
//    public double FontSize
//    {
//        get => (double)GetValue(FontSizeProperty);
//        set => SetValue(FontSizeProperty, value);
//    }

//    // line break mode
//    public static readonly BindableProperty LineBreakModeProperty = BindableProperty.Create(
//        nameof(LineBreakMode),
//        typeof(LineBreakMode),
//        typeof(LabelNative),
//        LineBreakMode.NoWrap
//    );
//    public LineBreakMode LineBreakMode
//    {
//        get => (LineBreakMode)GetValue(LineBreakModeProperty);
//        set => SetValue(LineBreakModeProperty, value);
//    }

//    // horizontal text alegnment
//    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
//        nameof(HorizontalTextAlignment),
//        typeof(TextAlignment),
//        typeof(LabelNative),
//        TextAlignment.Start
//    );
//    public TextAlignment HorizontalTextAlignment
//    {
//        get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
//        set => SetValue(HorizontalTextAlignmentProperty, value);
//    }

//    // vertical text alegnment
//    public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(
//        nameof(VerticalTextAlignment),
//        typeof(TextAlignment),
//        typeof(LabelNative),
//        TextAlignment.Start
//    );
//    public TextAlignment VerticalTextAlignment
//    {
//        get => (TextAlignment)GetValue(VerticalTextAlignmentProperty);
//        set => SetValue(VerticalTextAlignmentProperty, value);
//    }
//}

public enum LabelNativeAttributes
{
    None,
    NavigationTitle,
    AlertTitle,
    AlertDescription,
    AlertButton,
}