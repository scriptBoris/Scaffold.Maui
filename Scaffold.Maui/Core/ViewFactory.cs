﻿using ScaffoldLib.Maui.Containers;
using Material = ScaffoldLib.Maui.Containers.Material;
using WinUI = ScaffoldLib.Maui.Containers.WinUI;
using Cupertino = ScaffoldLib.Maui.Containers.Cupertino;
using ScaffoldLib.Maui.Args;

namespace ScaffoldLib.Maui.Core;

public class ViewFactory
{
    public Func<AgentArgs, IAgent>? OverrideAgent { get; set; }
    public Func<CreateNavigationBarArgs, INavigationBar>? OverrideNavigationBar { get; set; }
    public Func<CreateViewWrapperArgs, IViewWrapper>? OverrideViewWrapper { get; set; }
    public Func<CreateCollapsedMenuArgs, IZBufferLayout>? OverrideCollapsedMenu { get; set; }
    public Func<CreateDisplayAlertArgs, IDisplayAlert>? OverrideDisplayAlert { get; set; }
    public Func<CreateDisplayActionSheet, IDisplayActionSheet>? OverrideDisplayActionSheet { get; set; }
    public Func<CreateToastArgs, IToast>? OverrideToast { get; set; }

    internal IAgent CreateAgent(AgentArgs args)
    {
        var result = OverrideAgent?.Invoke(args);
        if (result == null)
        {
#if WINDOWS
            result = new WinUI.AgentWinUI(args);
#else
            result = new Containers.DefaultAgent(args);
#endif
        }
        OnAgentCreated(result);
        return result;
    }

    internal INavigationBar? CreateNavigationBar(CreateNavigationBarArgs args)
    {
        var result = OverrideNavigationBar?.Invoke(args);
        if (result == null)
        {
#if WINDOWS
            result = new WinUI.NavigationBar(args);
#elif IOS
            result = new Cupertino.NavigationBar(args);
#else
            result = new Material.NavigationBar(args);
#endif
        }
        OnNavigationBarCreated(result);
        return result;
    }

    internal IViewWrapper CreateViewWrapper(CreateViewWrapperArgs args)
    {
        var res = OverrideViewWrapper?.Invoke(args);
        if (res == null)
        {
#if WINDOWS
            res = new WinUI.ViewWrapperWinUI(args);
#else
            res = new ViewWrapper(args);
#endif
        }
        OnViewWrapperCreated(res);
        return res;
    }

    internal IZBufferLayout CreateCollapsedMenuItemsLayer(CreateCollapsedMenuArgs args)
    {
        var res = OverrideCollapsedMenu?.Invoke(args);
        if (res == null)
        {
#if WINDOWS
            res = new WinUI.CollapsedMenuItemLayer(args);
#elif IOS
            res = new Cupertino.CollapsedMenuItemLayer(args);
#else
            res = new Material.CollapsedMenuItemLayer(args);
#endif
        }
        OnCollapsedMenuCreated(res);
        return res;
    }

    internal IDisplayAlert CreateDisplayAlert(CreateDisplayAlertArgs args)
    {
        var res = OverrideDisplayAlert?.Invoke(args);
        if (res == null)
        {
#if IOS
            res = new Cupertino.DisplayAlertLayer(args);
#else
            res = new Material.DisplayAlertLayer(args);
#endif
        }
        OnDisplayAlertCreated(res);
        return res;
    }

    internal IDisplayActionSheet CreateDisplayActionSheet(CreateDisplayActionSheet args)
    {
        var res = OverrideDisplayActionSheet?.Invoke(args);
        if (res == null)
        {
#if IOS
            res = new Cupertino.DisplayActionSheetLayer(args);
#else
            res = new Material.DisplayActionSheetLayer(args);
#endif
        }
        OnDisplayActionSheetCreated(res);
        return res;
    }

    internal IToast? CreateToast(CreateToastArgs args)
    {
        var res = OverrideToast?.Invoke(args);
        if (res == null)
        {
            res = new Material.ToastLayer(args);
        }
        OnToastCreated(res);
        return res;
    }

    protected virtual void OnAgentCreated(IAgent agent) { }
    protected virtual void OnNavigationBarCreated(INavigationBar navigationBar) { }
    protected virtual void OnViewWrapperCreated(IViewWrapper viewWrapper) { }
    protected virtual void OnCollapsedMenuCreated(IZBufferLayout layout) { }
    protected virtual void OnDisplayAlertCreated(IDisplayAlert alert) { }
    protected virtual void OnDisplayActionSheetCreated(IDisplayActionSheet actionSheet) { }
    protected virtual void OnToastCreated(IToast toast) { }
}
