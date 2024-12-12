namespace ScaffoldLib.Maui.Core;

public interface IHardView
{
    Task ReadyToPush(CancellationToken cancellation);
}
