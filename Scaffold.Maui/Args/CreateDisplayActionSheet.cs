using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Args;

public class CreateDisplayActionSheet
{
    public string? Title { get; set; }
    public string? Cancel { get; set; }
    public string? Destruction { get; set; }
    public string? ItemDisplayBinding { get; set; }
    public required object[] Items { get; set; }
    public object? Payload { get; set; }
    public int? SelectedItemId { get; set; }
}
