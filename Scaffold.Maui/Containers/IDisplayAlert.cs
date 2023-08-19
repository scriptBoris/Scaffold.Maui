using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers;

public interface IDisplayAlert : IModalLayout
{
    Task<bool> GetResult();
}
