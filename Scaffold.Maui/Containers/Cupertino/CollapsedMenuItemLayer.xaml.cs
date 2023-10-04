using ButtonSam.Maui;
using ButtonSam.Maui.Core;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScaffoldLib.Maui.Containers.Cupertino;

public partial class CollapsedMenuItemLayer : IZBufferLayout
{
    private readonly TaskCompletionSource<bool> _tsc = new();

    private bool isBusy;
    public event VoidDelegate? DeatachLayer;

    public CollapsedMenuItemLayer(View view)
    {
        InitializeComponent();
        Padding = Scaffold.SafeArea;
        CommandSelectedMenu = new Command(ActionSelectedMenu);
        GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => Close().ConfigureAwait(false))
        });

        var obs = Scaffold.GetMenuItems(view).CollapsedItems;
        BindableLayout.SetItemsSource(stackMenu, obs);
    }

    public ICommand CommandSelectedMenu { get; private set; }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        _tsc.TrySetResult(true);
    }

    private void ActionSelectedMenu(object param)
    {
        if (param is MenuItem menuItem)
        {
            menuItem.Command?.Execute(null);
        }
        Close().ConfigureAwait(false);
    }

    public async Task Show()
    {
        isBusy = true;
        Opacity = 0;
        await _tsc.Task;
        await this.FadeTo(1, 180);
        isBusy = false;
    }

    public async Task Close()
    {
        if (isBusy)
            return;

        isBusy = true;

        await this.FadeTo(0, 180);
        DeatachLayer?.Invoke();
    }
}

public class ButtonCollapsedMenu : ButtonSam.Maui.Button
{
    private readonly StackLayout _stackLayout = new()
    {
        Orientation = StackOrientation.Horizontal,
        Spacing = 20,
    };
    private ImageTint? iconImage;
    private Label? label;
    private Color releasedAnimColor = Colors.Blue;
    private Color pressedAnimationColor = Colors.Blue;

    public ButtonCollapsedMenu()
    {
        Content = _stackLayout;
    }

    #region bindable props
    // icon
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(ImageSource),
        typeof(ButtonCollapsedMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCollapsedMenu self)
                self.UpdateIcon();
        }
    );
    public ImageSource? Icon
    {
        get => GetValue(IconProperty) as ImageSource;
        set => SetValue(IconProperty, value);
    }

    // text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ButtonCollapsedMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCollapsedMenu self)
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
        typeof(ButtonCollapsedMenu),
        Colors.Black,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCollapsedMenu self)
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
        typeof(ButtonCollapsedMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCollapsedMenu self)
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
        typeof(ButtonCollapsedMenu),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is ButtonCollapsedMenu self)
                self.UpdateColors();
        }
    );
    public bool UseOriginalColor
    {
        get => (bool)GetValue(UseOriginalColorProperty);
        set => SetValue(UseOriginalColorProperty, value);
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

    protected void AnimationPropertyColorIcon(Color color)
    {
        if (iconImage != null)
            iconImage.TintColor = color;
    }

    protected void AnimationPropertyColorLabel(Color color)
    {
        if (label != null)
            label.TextColor = color;
    }

    protected override void AnimationFrame(double x)
    {
        var colorIcon = releasedAnimColor.ApplyTint(pressedAnimationColor, x);
        AnimationPropertyColorIcon(colorIcon);

        var foreground = PriorityForegroundColor ?? ForegroundColor;
        var colorLabel = foreground.ApplyTint(TapColor, x);
        AnimationPropertyColorLabel(colorLabel);
    }

    //protected override Task<bool> MauiAnimationPressed()
    //{
    //    AnimationPropertyColor(pressedAnimationColor);
    //    return Task.FromResult(true);
    //}

    //protected override Task<bool> MauiAnimationReleased()
    //{
    //    AnimationPropertyColor(releasedAnimColor);
    //    return Task.FromResult(true);
    //}

    private void UpdateIcon()
    {
        if (Icon == null && iconImage != null)
        {
            _stackLayout.Children.Remove(iconImage);
            iconImage.Handler = null;
            iconImage = null;
        }
        else if (Icon != null && iconImage == null)
        {
            iconImage = new()
            {
                HeightRequest = 22,
                WidthRequest = 22,
            };
            _stackLayout.Children.Add(iconImage);
            UpdateColors();
        }

        if (iconImage != null)
            iconImage.Source = Icon;
    }

    private void UpdateText()
    {
        if (Text == null && label != null)
        {
            _stackLayout.Children.Remove(label);
            label.Handler = null;
            label = null;
        }
        else if (Text != null && label == null)
        {
            label = new Label
            {
                TextColor = ForegroundColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            _stackLayout.Children.Insert(0, label);
            UpdateColors();
        }

        if (label != null)
            label.Text = Text;
    }

    private void UpdateColors()
    {
        var foreground = PriorityForegroundColor ?? ForegroundColor;

        if (!UseOriginalColor)
        {
            AnimationPropertyColorLabel(foreground);
            AnimationPropertyColorIcon(foreground);
            releasedAnimColor = foreground;
            pressedAnimationColor = TapColor;
        }
        else
        {
            AnimationPropertyColorLabel(foreground);
            AnimationPropertyColorIcon(Colors.Transparent);
            releasedAnimColor = Colors.Transparent;
            pressedAnimationColor = TapColor.MultiplyAlpha(0.7f);
        }
    }
}