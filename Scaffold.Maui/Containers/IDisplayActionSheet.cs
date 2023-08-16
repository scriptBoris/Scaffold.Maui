using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers
{
    public interface IDisplayActionSheet : IZBufferLayout
    {
        Task<IDisplayActionSheetResult> GetResult();
    }
}
