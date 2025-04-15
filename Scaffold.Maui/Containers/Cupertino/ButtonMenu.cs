using Microsoft.Maui.Animations;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Toolkit;

#if IOS
using UIKit;
#endif

namespace ScaffoldLib.Maui.Containers.Cupertino;

public class ButtonMenu : StaticLibs.ButtonSam.Button
{
    private const double ButtonSize = 50;
    private ImageTint? _iconImage;
    private Label? _label;

    public ButtonMenu()
    {
        HeightRequest = ButtonSize;
        WidthRequest = ButtonSize;
    }

    #region bindable props
    // image source
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(ButtonMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
                self.UpdateIcon(n as ImageSource);
        }
    );
    public ImageSource? ImageSource
    {
        get => GetValue(ImageSourceProperty) as ImageSource;
        set => SetValue(ImageSourceProperty, value);
    }

    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ButtonMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
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
        typeof(ButtonMenu),
        Colors.Black,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
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
        typeof(ButtonMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
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
        typeof(ButtonMenu),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
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
        typeof(ButtonMenu),
        new Size(22, 22),
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
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

    // image source padding
    public static readonly BindableProperty ImageSourcePaddingProperty = BindableProperty.Create(
        nameof(ImageSourcePadding),
        typeof(Thickness),
        typeof(ButtonMenu),
        new Thickness(9),
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonMenu self)
                self.UpdateImagePaddings();
        }
    );
    public Thickness ImageSourcePadding
    {
        get => (Thickness)GetValue(ImageSourcePaddingProperty);
        set => SetValue(ImageSourcePaddingProperty, value);
    }
    #endregion bindable props

    protected override async Task<bool> MauiAnimationPressed()
    {
#if IOS
        var ui = (UIView)this.Handler!.PlatformView!;
        bool res = await UIView.AnimateAsync(0.070, () =>
        {
            ui.Alpha = 0.3f;
        });

        if (res)
            this.Opacity = 0.3;

        return res;
#else
        throw new NotImplementedException();
#endif
    }

    protected override async Task<bool> MauiAnimationReleased()
    {
#if IOS
        var ui = (UIView)this.Handler!.PlatformView!;
        bool res = await UIView.AnimateAsync(0.200, () =>
        {
            ui.Alpha = 1.0f;
        });

        if (res)
            this.Opacity = 1.0;

        return res;
#else
        throw new NotImplementedException();
#endif
    }

    protected override void AnimationPressedStop()
    {
#if IOS
        MauiAnimationReleased().ConfigureAwait(false);
#endif
    }

    private void UpdateText()
    {
        if (_iconImage == null)
        {
            if (Text == null && _label != null)
            {
                Content = null;
                _label.Handler = null;
                _label = null;
                return;
            }
            else if (Text != null && _label == null)
            {
                _label = new LabelNative
                {
                    TextColor = ForegroundColor,
                };
                Content = _label;
            }
        }

        if (_label != null)
            _label.Text = Text;
    }

    private void UpdateIcon(ImageSource? imageSource)
    {
        if (ImageSource == null && _iconImage != null)
        {
            Content = null;
            _iconImage.Handler = null;
            _iconImage = null;
            Padding = 0;
            UpdateText();
            return;
        }
        else if (ImageSource != null && _iconImage == null)
        {
            _iconImage = new ImageTint();
            Content = _iconImage;
            UpdateColors();
            UpdateImagePaddings();
        }

        if (_iconImage != null)
            _iconImage.Source = imageSource;
    }

    private void UpdateColors()
    {
        var textForeground = PriorityForegroundColor ?? ForegroundColor;
        var iconForeground = UseOriginalColor ? null : textForeground;

        if (_label != null)
            _label.TextColor = textForeground;
        
        if (_iconImage != null)
            _iconImage.TintColor = iconForeground;
    }

    private Size UpdateImagePaddings()
    {
        if (Content is not Image)
        {
            Padding = 0;
            return default;
        }

        double left = ImageSourcePadding.Left;
        double top = ImageSourcePadding.Top;
        double right = ImageSourcePadding.Right;
        double bottom = ImageSourcePadding.Bottom;

        var padding = new Thickness(left, top, right, bottom);
        Padding = padding;
        var size = new Size(
            ButtonSize - padding.HorizontalThickness,
            ButtonSize - padding.VerticalThickness);

        if (Content is ImageTint img)
        {
            img.HeightRequest = size.Height;
            img.WidthRequest = size.Width;
        }

        return size;
    }
}