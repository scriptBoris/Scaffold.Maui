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
        /// return True - if backbutton will be is override <br/>
        /// return False - scaffold himself resolve backbutton action
        /// </summary>
        Task<bool> OnBackButton();
    }
}
