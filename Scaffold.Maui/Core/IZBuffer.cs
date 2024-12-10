namespace ScaffoldLib.Maui.Core;

public interface IZBuffer
{
    IReadOnlyList<View> Layers { get; }
    Task AddLayerAsync(IZBufferLayout overlay, int zIndex, bool animation);
    void AddLayer(IZBufferLayout overlay, int zIndex, bool animation);
    Task RemoveLayer(IZBufferLayout overlay, bool animation);
    Task RemoveLayers(int zIndex, bool animation);

    /// <summary>
    /// Try pop modal layers (such as Dialogs, Alerts and etc) who impletents <see cref="IModalLayout"/>
    /// </summary>
    /// <returns>Return True - layer was removed, False - ZBuffer is already empty</returns>
    bool TryPopModal(bool animation);
}