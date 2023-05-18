using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers
{
    public interface IFrame
    {
        bool IsAppear { get; set; }
        INavigationBar? NavigationBar { get; }
        IViewWrapper ViewWrapper { get; }
        View? Overlay { get; set; }

        void DrawLayout();
        void UpdateSafeArea(Thickness safeArea);
        Task UpdateVisual(NavigatingArgs args);
    }
}
