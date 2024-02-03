using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core;

public interface IZBufferLayout : IView
{
    public event VoidDelegate? DeatachLayer;
    Task OnShow(CancellationToken cancel);
    Task OnHide(CancellationToken cancel);
    void OnRemoved();
}
