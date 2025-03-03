using ScaffoldLib.Maui.StaticLibs.ButtonSam.Core;
using ScaffoldLib.Maui.Toolkit;

namespace ScaffoldLib.Maui.Containers.Cupertino;

public class ButtonCupertino : StaticLibs.ButtonSam.Button
{
    private readonly ImageTint _iconImage;
    private Color releasedAnimColor = Colors.Blue;
    private Color pressedAnimationColor = Colors.Blue;

    public ButtonCupertino()
    {
        _iconImage = new ImageTint
        {
            HeightRequest = 22,
            WidthRequest = 22,
            Aspect = Aspect.AspectFill,
        };
        Content = _iconImage;
        UpdateColors();
    }

    #region bindable props
    // icon
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(ImageSource),
        typeof(ButtonCupertino),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCupertino self)
                self.UpdateIcon(n as ImageSource);
        }
    );
    public ImageSource? Icon
    {
        get => GetValue(IconProperty) as string;
        set => SetValue(IconProperty, value);
    }

    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ButtonCupertino),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCupertino self)
                self.UpdateText();
        }
    );
    public string? Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    // foreground color
    public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(
        nameof(ForegroundColor),
        typeof(Color),
        typeof(ButtonCupertino),
        Colors.Black,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCupertino self)
                self.UpdateColors();
        }
    );
    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }

    // priority foreground color
    public static readonly BindableProperty PriorityForegroundColorProperty = BindableProperty.Create(
        nameof(PriorityForegroundColor),
        typeof(Color),
        typeof(ButtonCupertino),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCupertino self)
                self.UpdateColors();
        }
    );
    public Color? PriorityForegroundColor
    {
        get => GetValue(PriorityForegroundColorProperty) as Color;
        set => SetValue(PriorityForegroundColorProperty, value);
    }

    // use original color
    public static readonly BindableProperty UseOriginalColorProperty = BindableProperty.Create(
        nameof(UseOriginalColor),
        typeof(bool),
        typeof(ButtonCupertino),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCupertino self)
                self.UpdateColors();
        }
    );
    public bool UseOriginalColor
    {
        get => (bool)GetValue(UseOriginalColorProperty);
        set => SetValue(UseOriginalColorProperty, value);
    }

    // icon size
    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSize),
        typeof(Size),
        typeof(ButtonCupertino),
        new Size(22, 22),
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCupertino self)
            {
                var size = (Size)n;
                self._iconImage.HeightRequest = size.Height;
                self._iconImage.WidthRequest = size.Width;
            }
        }
    );
    public Size IconSize
    {
        get => (Size)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    #endregion bindable props

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(TapColor))
        {
            UpdateColors();
        }
    }

    protected override void AnimationPropertyColor(Color color)
    {
        _iconImage.TintColor = color;
    }

    protected override void AnimationFrame(double x)
    {
        var color = releasedAnimColor.ApplyTint(pressedAnimationColor, x);
        AnimationPropertyColor(color);
    }

    protected override void AnimationMouseOverStop()
    {
        base.AnimationMouseOverStop();
        UpdateColors();
    }

    protected override void AnimationPressedStop()
    {
        base.AnimationPressedStop();
        UpdateColors();
    }

    protected override void RestoreButton()
    {
        base.RestoreButton();
        UpdateColors();
    }

    protected override Task<bool> MauiAnimationPressed()
    {
        AnimationPropertyColor(pressedAnimationColor);
        return Task.FromResult(true);
    }

    protected override Task<bool> MauiAnimationReleased()
    {
        AnimationPropertyColor(releasedAnimColor);
        return Task.FromResult(true);
    }

    private void UpdateIcon(ImageSource? imageSource)
    {
        _iconImage.Source = imageSource;
    }

    private void UpdateText()
    {
        //if (Text == null && label != null)
        //{
        //    _stackLayout.Children.Remove(label);
        //    label.Handler = null;
        //    label = null;
        //}
        //else if (Text != null && label == null)
        //{
        //    label = new Label
        //    {
        //        TextColor = ForegroundColor,
        //    };
        //    _stackLayout.Children.Add(label);
        //}

        //if (label != null)
        //    label.Text = Text;
    }

    private void UpdateColors()
    {
        var foreground = PriorityForegroundColor ?? ForegroundColor;

        if (!UseOriginalColor)
        {
            _iconImage.TintColor = foreground;
            releasedAnimColor = foreground;
            pressedAnimationColor = TapColor;
        }
        else
        {
            _iconImage.TintColor = null;
            releasedAnimColor = Colors.Transparent;
            pressedAnimationColor = TapColor.MultiplyAlpha(0.7f);
        }
    }
}