﻿using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal
{
    internal interface IPlatformSpecific
    {
        void SetStatusBarColorScheme(StatusBarColorTypes scheme);
        Thickness GetSafeArea();
    }
}