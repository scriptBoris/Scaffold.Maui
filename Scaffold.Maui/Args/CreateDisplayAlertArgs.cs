﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Args;

public interface ICreateDisplayAlertArgs
{
    string? Title { get; }
    string? Description { get; }
    string Ok { get; }
    string? Cancel { get; }
    object? Payload { get; }
}

public class CreateDisplayAlertArgs : ICreateDisplayAlertArgs
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public required string Ok { get; set; }
    public object? Payload { get; set; }

    string? ICreateDisplayAlertArgs.Cancel => null;
}
