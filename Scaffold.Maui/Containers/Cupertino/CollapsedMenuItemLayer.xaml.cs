using ButtonSam.Maui;
using ButtonSam.Maui.Core;
using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Toolkit;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScaffoldLib.Maui.Containers.Cupertino;

public partial class CollapsedMenuItemLayer : IZBufferLayout
{
    public event VoidDelegate? DeatachLayer;

    public CollapsedMenuItemLayer(CreateCollapsedMenuArgs args)
    {
        InitializeComponent();
        Padding = Scaffold.DeviceSafeArea;
        border.Scale = 0;
        border.AnchorX = 1;
        border.AnchorY = 0;
        CommandSelectedMenu = new Command(ActionSelectedMenu);
        GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => DeatachLayer?.Invoke())
        });

        var obs = Scaffold.GetMenuItems(args.View).CollapsedItems;
        BindableLayout.SetItemsSource(stackMenu, obs);
    }

    public ICommand CommandSelectedMenu { get; private set; }

    private void ActionSelectedMenu(object param)
    {
        if (param is ScaffoldMenuItem menuItem)
        {
            menuItem.Command?.Execute(null);
        }
        DeatachLayer?.Invoke();
    }

    public async Task OnShow(CancellationToken cancel)
    {
        await border.ScaleTo(1, 200, Easing.SpringOut);
    }

    public async Task OnHide(CancellationToken cancel)
    {
        await border.ScaleTo(0, 200, Easing.SinInOut);
    }

    public void OnShow()
    {
        border.Scale = 1;
    }

    public void OnHide()
    {
        border.Scale = 0;
    }

    public void OnRemoved()
    {
        border.ScaleTo(0, 200, Easing.SinInOut);
    }

    public void OnTapToOutside()
    {
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

    protected override void AnimationFrame(double x)
    {
        BackgroundColor = Colors.Gray;
    }

    protected override void RestoreButton()
    {
        BackgroundColor = Colors.Transparent;
    }

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
            if (label != null)
                label.TextColor = foreground;

            if (iconImage != null)
                iconImage.TintColor = foreground;
        }
        else
        {
            if (label != null)
                label.TextColor = foreground;

            if (iconImage != null)
                iconImage.TintColor = null;
        }
    }
}