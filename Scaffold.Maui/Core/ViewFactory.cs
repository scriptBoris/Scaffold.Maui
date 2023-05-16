using Microsoft.Maui.Controls;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Material = ScaffoldLib.Maui.Containers.Material;

namespace ScaffoldLib.Maui.Core
{
    public class ViewFactory
    {
        public virtual IFrame CreateFrame(View view)
        {
            return new Internal.Frame(view, this);
        }

        public virtual INavigationBar? CreateNavigationBar(View view)
        {
            return new Material.NavigationBar(view);
        }

        public virtual IViewWrapper CreateViewWrapper(View view)
        {
            return new ViewWrapper(view);
        }

        public virtual IDisplayAlert CreateDisplayAlert(string title, string message, string ok)
        {
            return new Material.DisplayAlertLayer(title, message, ok);
        }

        public virtual IDisplayAlert CreateDisplayAlert(string title, string message, string ok, string cancel)
        {
            return new Material.DisplayAlertLayer(title, message, ok, cancel);
        }

        public virtual IZBufferLayout CreateCollapsedMenuItemsLayer(View view)
        {
            return new Material.CollapsedMenuItemLayer(view);
        }
    }
}
