using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Args;

public class CreateCollapsedMenuArgs
{
    public required View View { get; set; }
    public required IScaffold Context {  get; set; }
}
