namespace ScaffoldLib.Maui.Core;

public interface IZBuffer
{
    IReadOnlyList<View> Layers { get; }
    Task AddLayerAsync(IZBufferLayout overlay, int zIndex, bool animation);
    void AddLayer(IZBufferLayout overlay, int zIndex, bool animation);
    Task RemoveLayer(IZBufferLayout overlay, bool animation);
    Task RemoveLayers(int zIndex, bool animation);

    /// <summary>
    /// Получает первый по z индексу модальный попап
    /// </summary>
    /// <returns></returns>
    IZBufferLayout? GetActualModalLayer();

    /// <summary>
    /// Try pop modal layers (such as Dialogs, Alerts and etc) who impletents <see cref="IModalLayout"/>
    /// </summary>
    /// <returns>
    /// <c>true</c> - modal layer was removed
    /// <br/>
    /// <c>false</c> - modal layer is super modal, that implemented <see cref="IBackButtonListener"/>
    /// <br/>
    /// <c>null</c> - there are no modal elements that can be removed
    /// </returns>
    Task<bool?> TryPopModal(bool animation);
}