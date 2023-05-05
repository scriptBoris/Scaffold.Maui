using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core
{
    public delegate void VoidDelegate();

    public interface IZBufferLayout : IView
    {
        public event VoidDelegate? DeatachLayer;
        Task Show();
        Task Close();
    }
}
