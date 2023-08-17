using Microsoft.Maui.Controls;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Containers.WinUI;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Material = ScaffoldLib.Maui.Containers.Material;
using WinUI = ScaffoldLib.Maui.Containers.WinUI;

namespace ScaffoldLib.Maui.Core;

public class ViewFactory
{
    public virtual IFrame CreateFrame(View view, IScaffold context)
    {
#if WINDOWS
        return new WinUI.FrameWinUI(view, context);
#else
        return new Containers.Frame(view, context);
#endif
    }

    public virtual INavigationBar? CreateNavigationBar(View view, IScaffold context)
    {
#if WINDOWS
        return new WinUI.NavigationBar(view);
#else
        return new Material.NavigationBar(view);
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

    public virtual IDisplayAlert CreateDisplayAlert(string title, string message, string ok, IScaffold context)
    {
        return new Material.DisplayAlertLayer(title, message, ok);
    }

    public virtual IDisplayAlert CreateDisplayAlert(string title, string message, string ok, string cancel, IScaffold context)
    {
        return new Material.DisplayAlertLayer(title, message, ok, cancel);
    }

    public virtual IZBufferLayout CreateCollapsedMenuItemsLayer(View view, IScaffold context)
    {
        return new Material.CollapsedMenuItemLayer(view);
    }

    public virtual IDisplayActionSheet? CreateDisplayActionSheet(string? title, string? cancel, string? destruction, string[] buttons)
    {
        if (buttons.Count() == 0)
            return null;

        return new Material.DisplayActionSheetLayer(title, cancel, destruction, buttons);
    }

    public virtual IToast? CreateToast(string? title, string message, TimeSpan showTime)
    {
        return new Material.ToastLayer(title, message, showTime);
    }
}
