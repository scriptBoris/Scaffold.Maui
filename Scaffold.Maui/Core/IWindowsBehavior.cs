using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core;

public interface IWindowsBehavior
{
    // TODO Переделать на IEnumerable, и реализацию через yield return, чтобы уменьшить аллокации
    Rect[] UndragArea { get; }
}
