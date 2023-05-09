using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Core
{
    public interface IBackButtonListener
    {
        /// <summary>
        /// return True - view will be closed when user click software/hardware backbutton <br/>
        /// return False - view will not closed when user click software/hardware backbutton 
        /// </summary>
        Task<bool> OnBackButton();
    }
}
