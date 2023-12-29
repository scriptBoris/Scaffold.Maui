using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Args;

public class CreateAgentArgs
{
    public required IScaffold Context { get; set; }
    public required View View { get; set; }
    public required int IndexInStack { get; set; }
    public required Color NavigationBarBackgroundColor { get; set; }
    public required Color NavigationBarForegroundColor { get; set; }
    public required Thickness SafeArea { get; set; }
    public required IBackButtonBehavior? BackButtonBehavior { get; set; }
    public required IBehavior[] Behaviors { get; set; }
}
