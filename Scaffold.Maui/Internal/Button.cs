using ButtonSam.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

internal class Button : ButtonSam.Maui.Button
{
    protected override void AnimationPressedStart(float x, float y)
    {
        float newX = (float)Width / 2f;
        float newY = (float)Height / 2f;
        base.AnimationPressedStart(newX, newY);
    }

    //private void OnGestureMove_origin(InteractiveEventArgs args)
    //{
    //    float num = Math.Abs(StartX - args.X);
    //    float num2 = Math.Abs(StartY - args.Y);
    //    if (num > 40f || num2 > 40f)
    //    {
    //        IsPressed = false;
    //    }

    //    if (args.DeviceInputType == DeviceInputTypes.Mouse)
    //    {
    //        IsMouseOver = new Rect(0.0, 0.0, base.Frame.Width, base.Frame.Height).Contains(args.X, args.Y);
    //    }
    //    else
    //    {
    //        IsMouseOver = false;
    //    }
    //}

    //protected override void OnGestureMove(InteractiveEventArgs args)
    //{
    //    bool isMouseOver = base.IsMouseOver;
    //    bool isPressed = base.IsPressed;
    //    OnGestureMove_origin(args);
    //    if (isPressed != base.IsPressed)
    //    {
    //        OnAnimationFinish();
    //    }
    //    else if (!base.IsPressed && !IsRippleEffectSupport && isMouseOver != base.IsMouseOver)
    //    {
    //        if (base.IsMouseOver && base.IsEnabled)
    //        {
    //            AnimationMouseOverStart();
    //        }
    //        else if (!base.IsMouseOver)
    //        {
    //            AnimationMouseOverRestore();
    //        }
    //    }
    //}

    //protected override void OnGesturePressed(InteractiveEventArgs args)
    //{
    //    args = new InteractiveEventArgs
    //    {
    //        DeviceInputType = args.DeviceInputType,
    //        InputType = args.InputType,
    //        State = args.State,
    //        X = (float)Width / 2f,
    //        Y = (float)Height / 2f,
    //    };
    //    base.OnGesturePressed(args);
    //}
}
