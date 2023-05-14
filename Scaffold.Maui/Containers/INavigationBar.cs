using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers;

public interface INavigationBar
{
    string? Title { get; set; }
    Task UpdateVisual(NavigatingArgs args);
    void UpdateMenuItems(View view);
    void UpdateNavigationBarVisible(bool visible);
    void UpdateNavigationBarBackgroundColor(Color color);
    void UpdateNavigationBarForegroundColor(Color color);
    void UpdateBackButtonBehavior(IBackButtonBehavior? behavior);
}


