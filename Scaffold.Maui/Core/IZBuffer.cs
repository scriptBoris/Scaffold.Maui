namespace ScaffoldLib.Maui.Core;

public interface IZBuffer
{
    void AddLayer(IZBufferLayout overlay, int menuItemsIndexZ);
    bool Pop();
}