using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit;

public class RadioButton : global::Microsoft.Maui.Controls.RadioButton
{
    // checked color
    public static readonly BindableProperty CheckedColorProperty = BindableProperty.Create(
        nameof(CheckedColor),
        typeof(Color),
        typeof(RadioButton),
        Colors.White
    );
    public Color CheckedColor
    {
        get => (Color)GetValue(CheckedColorProperty);
        set => SetValue(CheckedColorProperty, value);
    }

    // unchecked color
    public static readonly BindableProperty UncheckedColorProperty = BindableProperty.Create(
        nameof(UncheckedColor),
        typeof(Color),
        typeof(RadioButton),
        Colors.White
    );
    public Color UncheckedColor
    {
        get => (Color)GetValue(UncheckedColorProperty);
        set => SetValue(UncheckedColorProperty, value);
    }
}