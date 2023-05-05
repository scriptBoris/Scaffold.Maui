using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core
{
    public interface IScaffoldProvider
    {
        IScaffold? ProvideScaffold { get; }
    }
}
