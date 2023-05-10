using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core
{
    public interface IAppear
    {
        event EventHandler<bool>? Appear;
        void OnAppear(bool isComplete);
    }

    public interface IDisappear
    {
        event EventHandler<bool>? Disappear;
        void OnDisappear(bool isComplete);
    }
}
