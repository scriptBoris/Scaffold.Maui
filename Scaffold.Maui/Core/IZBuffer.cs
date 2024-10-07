namespace ScaffoldLib.Maui.Core;

public interface IZBuffer
{
    IReadOnlyList<View> Layers { get; }
    void AddLayer(IZBufferLayout overlay, int menuItemsIndexZ);
    bool Pop();
}