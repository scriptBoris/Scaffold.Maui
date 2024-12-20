namespace ScaffoldLib.Maui.Args;

public class DisplayActionSheet
{
    public string? Title { get; set; }
    public string? Cancel { get; set; }
    public string? Destruction { get; set; }
    public string? ItemDisplayBinding { get; set; }
    public required object[] Items { get; set; }
    public object? Payload { get; set; }
    public int? SelectedItemId { get; set; }
    public bool UseShowAnimation { get; set; } = true;
    public View? ParentView { get; set; }
}