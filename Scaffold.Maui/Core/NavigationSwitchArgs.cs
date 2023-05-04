using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core
{
    public class NavigationSwitchArgs
    {
        public required View NewContent { get; set; }
        public required bool HasBackButton { get; set; }
        public required bool IsAnimating { get; set; } = true;
        public IntentType ActionType { get; set; }
    }

    public enum IntentType
    {
        Push,
        Pop,
    }
}
