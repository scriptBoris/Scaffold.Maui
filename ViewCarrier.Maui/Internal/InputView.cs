using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scaffold.Maui.Internal
{
    internal class InputView : ContentView
    {
        public InputView()
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (o, e) =>
            {
                CommandSimpleTap?.Execute(null);
            };
            GestureRecognizers.Add(tap);
        }

        public ICommand? CommandSimpleTap { get; set; }
    }
}
