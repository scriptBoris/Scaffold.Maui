using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers
{
    public interface IDisplayActionSheet<T> : IModalLayout
    {
        Task<IDisplayActionSheetResult<T>> GetResult();
    }

    public interface IDisplayActionSheet : IModalLayout
    {
        Task<IDisplayActionSheetResult> GetResult();
    }
}
