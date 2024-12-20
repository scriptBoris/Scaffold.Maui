namespace ScaffoldLib.Maui.Core;

public delegate void SharedModalBackgroundTapped(ISharedModalBackground invoker, EventArgs args);

public interface ISharedModalBackground : IZBufferLayout
{
    event SharedModalBackgroundTapped? TappedToOutside;
    int ZBufferIndex { get; set; }
    void IZBufferLayout.OnTapToOutside() { }
}