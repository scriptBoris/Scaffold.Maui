using Scaffold.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Containers;

public interface INavigationBar : IView, IDisposable
{
    string? Title { get; set; }
    Task UpdateVisual(NavigatingArgs args);
    void UpdateMenu(View view);
}


