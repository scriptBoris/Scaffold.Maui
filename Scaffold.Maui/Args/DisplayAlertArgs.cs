﻿namespace ScaffoldLib.Maui.Args;

public class DisplayAlertArgs
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public required string Ok { get; set; }
    public object? Payload { get; set; }
    public bool UseShowAnimation { get; set; } = true;
    public View? ParentView { get; set; }
}