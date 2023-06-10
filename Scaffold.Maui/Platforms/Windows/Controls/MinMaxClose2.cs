using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Windows.Controls;

internal class MinMaxClose2 : global::Microsoft.UI.Xaml.Controls.Grid
{
    public MinMaxClose2(IMauiContext context)
    {
        var label = new Label().ToPlatform(context);
        Children.Add(label);
    }
}
