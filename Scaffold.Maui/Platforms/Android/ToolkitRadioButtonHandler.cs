using Android.Content;
using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Android;

public class ToolkitRadioButtonHandler : RadioButtonHandler
{
    public static PropertyMapper<IRadioButton, IRadioButtonHandler> Mapper2 = new(RadioButtonHandler.Mapper)
    {
        [nameof(Toolkit.RadioButton.CheckedColor)] = MapColor,
        [nameof(Toolkit.RadioButton.UncheckedColor)] = MapColor,
        [nameof(VisualElement.BackgroundColor)] = MapBackgroundColor,
    };

    public ToolkitRadioButtonHandler() : base(Mapper2)
    {
    }

    protected override AppCompatRadioButton CreatePlatformView()
    {
        return new AppCompatRadioButton2(Context)
        {
            SoundEffectsEnabled = false
        };
    }

    private static void MapBackgroundColor(IRadioButtonHandler h, IRadioButton view)
    {
        var v = (Toolkit.RadioButton)view;
        var platform = h.PlatformView as AppCompatRadioButton2;
        if (platform != null)
        {
            platform.HasBackgroundColor = v.BackgroundColor != null;
            if (v.BackgroundColor == null)
            {
                platform.Background = null;
            }
            else
            {
                RadioButtonHandler.MapBackground(h, view);
            }
        }
    }

    private static void MapColor(IRadioButtonHandler h, IRadioButton view)
    {
        var v = (Toolkit.RadioButton)view;
        var platform = h.PlatformView as global::Android.Widget.RadioButton;
        if (platform != null)
        {
            int[] colors =
            [
                v.CheckedColor.ToPlatform(),
                v.UncheckedColor.ToPlatform(),
            ];

            int[][] states =
            [
                [global::Android.Resource.Attribute.StateChecked], // Активное состояние
                [-global::Android.Resource.Attribute.StateChecked] // Неактивное состояние
            ];

            var colorStateList = new global::Android.Content.Res.ColorStateList(states, colors);
            platform.ButtonTintList = colorStateList;
        }
    }

    private class AppCompatRadioButton2 : AppCompatRadioButton
    {
        public AppCompatRadioButton2(Context? context) : base(context)
        {
        }

        // Переопределяем задний фон, т.к. у net maui8 сломанное поведение:
        // когда неопределен BackgroundColor, то идет установка заднего фона 
        // путен установки BorderDrawable из:
        // Microsoft.Maui.Platform.RadioButtonExtensions.UpdateBorderDrawable (46;5)
        // Поэтому используем костыль, который устанавливает прозрачный Background,
        // если HasBackgroundColor - false
        public override Drawable? Background
        {
            get => base.Background;
            set => base.Background = TrySetBackground(value);
        }

        public bool HasBackgroundColor { get; set; }

        private Drawable? TrySetBackground(Drawable? background)
        {
            if (HasBackgroundColor)
            {
                return background;
            }

            return new ColorDrawable(global::Android.Graphics.Color.Transparent);
        }
    }
}