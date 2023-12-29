using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Args;

public class CreateToastArgs
{
    public string? Title { get; set; }
    public required string Message { get; set; }
    public required TimeSpan ShowTime { get; set; }
    public object? Payload { get; set; }
}
