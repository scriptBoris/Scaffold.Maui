using ScaffoldLib.Maui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScaffoldLib.Maui.Toolkit;
#if WINDOWS
using BaseView = Microsoft.Maui.Controls.View;
#else
using BaseView = ButtonSam.Maui.Button;
#endif

namespace SamplePizza.Controls;

public class ElementPicker : BaseView
{
#if !WINDOWS
    private readonly Label label;
    public ElementPicker()
    {
        label = new Label()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalTextAlignment = TextAlignment.Center,
        };
        var stack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
        };
        stack.Children.Add(label);

        var icon = new ImageTint
        {
            Source = "chevron_down.png",
        };
        icon.SetAppThemeColor(ImageTint.TintColorProperty, Color.FromRgba("#333"), Color.FromRgba("#EEE"));
        stack.Children.Add(icon);
        Content = stack;

        this.SetAppThemeColor(View.BackgroundColorProperty, Color.FromRgba("#FFF"), Color.FromRgba("#333"));
        this.BorderWidth = 0.5;
        this.BorderColor = Colors.Gray;
        this.CornerRadius = 6;
    }
#endif

    #region bindable props
    // items source
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(object),
        typeof(ElementPicker),
        null
    );
    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    // selected item
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(object),
        typeof(ElementPicker),
        null,
        defaultBindingMode: BindingMode.TwoWay
    );
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    // display property
    public static readonly BindableProperty DisplayPropertyProperty = BindableProperty.Create(
        nameof(DisplayProperty),
        typeof(string),
        typeof(ElementPicker),
        null
    );
    public string? DisplayProperty
    {
        get => GetValue(DisplayPropertyProperty) as string;
        set => SetValue(DisplayPropertyProperty, value);
    }

    // placeholder
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(ElementPicker),
        null
    );
    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty) as string;
        set => SetValue(PlaceholderProperty, value);
    }
    #endregion bindable props

#if !WINDOWS
    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(SelectedItem):
            case nameof(Placeholder):
            case nameof(DisplayProperty):
                UpdateLabel();
                break;

            default:
                break;
        }
    }

    protected override async void OnTapComleted()
    {
        base.OnTapComleted();

        var items = new List<object>();
        if (ItemsSource is IEnumerable src)
        {
            foreach (var item in src)
                items.Add(item);
        }

        var res = await Scaffold.GetRootScaffold()!.DisplayActionSheet(new ScaffoldLib.Maui.Args.CreateDisplayActionSheet
        {
            Title = Placeholder,
            Cancel = "Cancel",
            Items = items.ToArray(),
        });
        if (res.IsDestruction)
        {
            SelectedItem = null;
        }
        else if (res.HasSelectedItem)
        {
            SelectedItem = res.SelectedItem;
        }
    }

    private void UpdateLabel()
    {
        if (SelectedItem != null)
        {
            label.Text = SelectedItem.GetDisplayItemText(DisplayProperty);
        }
        else
        {
            label.Text = Placeholder;
        }
    }
#endif
}
