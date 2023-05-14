using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core
{
    public class NavigatingArgs
    {
        public required View NewContent { get; set; }
        public required View? OldContent { get; set; }
        public required bool HasBackButton { get; set; }
        public required bool IsAnimating { get; set; }
        public required Color NavigationBarBackgroundColor { get; set; }
        public required Color NavigationBarForegroundColor { get; set; }
        public required Thickness SafeArea { get; set; }
        public required NavigatingTypes NavigationType { get; set; }
    }

    public enum NavigatingTypes
    {
        Push,
        Pop,
        Replace,
    }
}
