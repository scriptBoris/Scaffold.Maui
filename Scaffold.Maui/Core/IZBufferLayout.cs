using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core
{
    public delegate void VoidDelegate();
    public delegate void SingleDelegate<T>(T arg);

    public interface IZBufferLayout : IView
    {
        public event VoidDelegate? DeatachLayer;
        Task Show();
        Task Close();
    }
}
