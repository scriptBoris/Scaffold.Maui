using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core
{
    public interface IAppear
    {
        void OnAppear(bool isComplete);
    }

    public interface IDisappear
    {
        void OnDisappear(bool isComplete);
    }
}
