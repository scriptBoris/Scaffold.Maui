using ScaffoldLib.Maui.Containers;
using Material = ScaffoldLib.Maui.Containers.Material;
using WinUI = ScaffoldLib.Maui.Containers.WinUI;

namespace ScaffoldLib.Maui.Core;

public class ViewFactory
{
    public virtual IAgent CreateAgent(AgentArgs args, IScaffold context)
    {
#if WINDOWS
        return new WinUI.AgentWinUI(args, context);
#else
        return new Containers.DefaultAgent(args, context);
#endif
    }

    public virtual INavigationBar? CreateNavigationBar(View view, IAgent agent)
    {
#if WINDOWS
        return new WinUI.NavigationBar(view, agent);
#else
        return new Material.NavigationBar(view, agent);
#endif
    }

    public virtual IViewWrapper CreateViewWrapper(View view, IScaffold context)
    {
#if WINDOWS
        return new WinUI.ViewWrapperWinUI(view);
#else
        return new ViewWrapper(view);
#endif
    }

    public virtual IZBufferLayout CreateCollapsedMenuItemsLayer(View view, IScaffold context)
    {
#if WINDOWS
        return new WinUI.CollapsedMenuItemLayer(view);
#else
        return new Material.CollapsedMenuItemLayer(view);
#endif
    }

    public virtual IDisplayAlert CreateDisplayAlert(string title, string message, string ok, IScaffold context)
    {
        return new Material.DisplayAlertLayer(title, message, ok);
    }

    public virtual IDisplayAlert CreateDisplayAlert(string title, string message, string ok, string cancel, IScaffold context)
    {
        return new Material.DisplayAlertLayer(title, message, ok, cancel);
    }

    public virtual IDisplayActionSheet CreateDisplayActionSheet(string? title, string? cancel, string? destruction, string[] buttons)
    {
        return new Material.DisplayActionSheetLayer(title, cancel, destruction, buttons);
    }

    public virtual IToast? CreateToast(string? title, string message, TimeSpan showTime)
    {
        return new Material.ToastLayer(title, message, showTime);
    }
}
