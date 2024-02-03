using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit;

public static class StaticTools
{
    public static readonly BindableProperty InputTransparentProperty = BindableProperty.CreateAttached(
        "TK_InputTransparent",
        typeof(bool?),
        typeof(StaticTools),
        null,
        propertyChanged: async (b, o, n) =>
        {
            if (n == null)
                return;

            bool value = (bool)n;
            if (b is View v)
            {
#if IOS
                var h = await v.AwaitHandler();
                if (h?.PlatformView is UIKit.UIView uiv)
                    uiv.UserInteractionEnabled = !value;
#else
                v.InputTransparent = value;
#endif
            }
        });
    public static void SetInputTransparent(BindableObject bindable, bool? value) =>
        bindable.SetValue(InputTransparentProperty, value);
    public static bool? GetInputTransparent(BindableObject bindable) =>
        bindable.GetValue(InputTransparentProperty) as bool?;
}