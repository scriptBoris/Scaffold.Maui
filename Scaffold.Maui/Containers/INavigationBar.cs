using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Containers;

public interface INavigationBar
{
    void UpdateMenuItems(IList<ScaffoldMenuItem>? menuItems);
    void UpdateTitle(string? title);
    void UpdateNavigationBarVisible(bool visible);
    void UpdateNavigationBarBackgroundColor(Color color);
    void UpdateNavigationBarForegroundColor(Color color);
    void UpdateBackButtonVisibility(bool isVisible);
    void UpdateBackButtonBehavior(IBackButtonBehavior? behavior);
    void UpdateSafeArea(Thickness safeArea);
    void UpdateTitleView(View? titleView);
}