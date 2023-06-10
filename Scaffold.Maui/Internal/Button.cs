using ButtonSam.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

internal class Button : ButtonSam.Maui.Button
{
    //protected override void HandleInteractiveRunning(HandleInteractiveRunningArgs args)
    //{
    //    if (args.Input.X < 0 || args.Input.X > Width) 
    //    {
    //        args.IsPressed = false;
    //        return;
    //    }

    //    if (args.Input.Y < 0 || args.Input.Y > Height)
    //    {
    //        args.IsPressed = false;
    //        return;
    //    }
    //}

    protected override void OnGesturePressed(InteractiveEventArgs args)
    {
        args = new InteractiveEventArgs
        {
            DeviceInputType = args.DeviceInputType,
            InputType = args.InputType,
            State = args.State,
            X = (float)Width / 2f,
            Y = (float)Height / 2f,
        };
        base.OnGesturePressed(args);
    }
}
