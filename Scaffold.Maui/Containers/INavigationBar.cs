using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers;

public interface INavigationBar
{
    Task UpdateVisual(NavigatingArgs args);
    void UpdateMenuItems(View view);
    void UpdateTitle(string? title);
    void UpdateNavigationBarVisible(bool visible);
    void UpdateNavigationBarBackgroundColor(Color color);
    void UpdateNavigationBarForegroundColor(Color color);
    void UpdateBackButtonBehavior(IBackButtonBehavior? behavior);
    void UpdateSafeArea(Thickness safeArea);
}


