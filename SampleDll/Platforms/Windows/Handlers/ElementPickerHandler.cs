using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDll.Controls;

public class ElementPickerHandler : ViewHandler<ElementPicker, ComboBox>
{
    public ElementPickerHandler() : base(PropertyMapper)
    {
    }

    public static PropertyMapper<ElementPicker, ElementPickerHandler> PropertyMapper = new(ViewMapper)
    {
        [nameof(ElementPicker.ItemsSource)] = MapItemsSource,
        [nameof(ElementPicker.Placeholder)] = MapPlaceholder,
        [nameof(ElementPicker.SelectedItem)] = MapSelectedItem,
    };

    private static void MapItemsSource(ElementPickerHandler h, ElementPicker v)
    {
        h.PlatformView.ItemsSource = h.VirtualView.ItemsSource;
    }

    private static void MapPlaceholder(ElementPickerHandler h, ElementPicker v)
    {
        h.PlatformView.PlaceholderText = h.VirtualView.Placeholder;
    }

    private static void MapSelectedItem(ElementPickerHandler h, ElementPicker v)
    {
        h.PlatformView.SelectedItem = h.VirtualView.SelectedItem;
    }

    protected override ComboBox CreatePlatformView()
    {
        var native = new ComboBox();
        native.SelectionChanged += Native_SelectionChanged;
        Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged += Current_RequestedThemeChanged;
        return native;
    }

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        ResolveTheme(Microsoft.Maui.Controls.Application.Current!.RequestedTheme);
    }

    protected override void DisconnectHandler(ComboBox platformView)
    {
        base.DisconnectHandler(platformView);
        platformView.SelectionChanged -= Native_SelectionChanged;
        Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged -= Current_RequestedThemeChanged;
    }

    private void ResolveTheme(AppTheme requestedTheme)
    {
        switch (requestedTheme)
        {
            case AppTheme.Light:
                PlatformView.RequestedTheme = ElementTheme.Light;
                break;
            case AppTheme.Dark:
                PlatformView.RequestedTheme = ElementTheme.Dark;
                break;
            default:
                PlatformView.RequestedTheme = ElementTheme.Default;
                break;
        }
    }

    private void Native_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        VirtualView.SelectedItem = PlatformView.SelectedItem;
    }

    private void Current_RequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ResolveTheme(e.RequestedTheme);
    }
}
