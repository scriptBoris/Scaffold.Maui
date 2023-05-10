using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core
{
    public interface IRemovedFromNavigation
    {
        event EventHandler? RemovedFromNavigation;
        void OnRemovedFromNavigation();
    }
}
