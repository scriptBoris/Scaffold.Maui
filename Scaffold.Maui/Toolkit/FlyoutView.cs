using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Toolkit;

[ContentProperty(nameof(Flyout))]
public class FlyoutView :
#if WINDOWS
    FlyoutViewPlatforms.FlyoutViewWinUI
#else
    FlyoutViewPlatforms.FlyoutViewMaterial
#endif
{
}
