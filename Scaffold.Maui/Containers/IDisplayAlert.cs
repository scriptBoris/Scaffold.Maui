using Scaffold.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Containers
{
    public interface IDisplayAlert : IZBufferLayout
    {
        Task<bool> GetResult();
    }
}
