using Scaffold.Maui.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core
{
    public interface IBackButtonBehavior
    {
        bool? OverrideSoftwareBackButtonAction(IScaffold context);
        bool? OverrideHardwareBackButtonAction(IScaffold context);

        bool? OverrideBackButtonVisibility(IScaffold context);
        ImageSource? OverrideBackButtonIcon(IScaffold context);
    }
}
