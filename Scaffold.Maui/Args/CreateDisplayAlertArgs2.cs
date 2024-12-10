using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Args;

public class CreateDisplayAlertArgs2 : ICreateDisplayAlertArgs
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public required string Ok { get; set; }
    public required string Cancel { get; set; }
    public object? Payload { get; set; }
}
