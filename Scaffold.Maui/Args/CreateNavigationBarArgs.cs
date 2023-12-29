using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Args;

public class CreateNavigationBarArgs
{
    public required View View { get; set; }
    public required IAgent Agent { get; set; }
}
