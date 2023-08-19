using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers
{
    public interface IDisplayActionSheet : IModalLayout
    {
        Task<IDisplayActionSheetResult> GetResult();
    }
}
