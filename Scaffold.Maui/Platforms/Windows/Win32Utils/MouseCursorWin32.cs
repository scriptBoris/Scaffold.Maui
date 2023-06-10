using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Platforms.Windows.Win32Utils;

public static class MouseCursorWin32
{
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    public static POINT Position
    {
        get
        {
            GetCursorPos(out POINT point);
            return point;
        }
        set
        {
            SetCursorPos(value.X, value.Y);
        }
    }

    public static Vector2 GetPositionAsVector2()
    {
        var p = Position;
        return new Vector2(p.X, p.Y);
    }

    public static void ChangeCursor(UIElement uiElement, InputCursor cursor)
    {
        Type type = typeof(UIElement);
        type.InvokeMember("ProtectedCursor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, uiElement, new object[] { cursor });
    }
}
