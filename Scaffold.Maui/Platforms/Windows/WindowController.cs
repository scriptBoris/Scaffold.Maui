using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Platforms.Windows.Controls;
using ScaffoldLib.Maui.Platforms.Windows.Win32Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
using static ScaffoldLib.Maui.Internal.WindowsMinMaxClose;

namespace ScaffoldLib.Maui.Platforms.Windows;

internal class WindowController
{
    private readonly WindowsMinMaxClose _minMaxClose;
    private readonly UIElement _mainWindowContent;
    private readonly MauiWinUIWindow _window;
    private readonly IScaffold _scaffold;

    private Microsoft.UI.Xaml.Input.Pointer? pointerMove;
    private Microsoft.UI.Xaml.Input.Pointer? pointerClick;
    private CoreCursorType pressedMouseState = CoreCursorType.Arrow;
    private ResizeDirections resizeDirection = ResizeDirections.None;

    private Vector2 lastMousePoint;
    private global::Windows.Foundation.Point windowOffset;
    private bool isMaxizedWindow;
    private Rect lastWindowsLocation;

    public WindowController(MauiWinUIWindow window, IScaffold scaffold, Core.StatusBarColorTypes initialColorScheme)
    {
        _window = window;
        _scaffold = scaffold;

        var rootPanel = (Microsoft.UI.Xaml.Controls.Panel)window.Content;
        var context = Microsoft.Maui.Controls.Application.Current!.Handler.MauiContext;
        _minMaxClose = new WindowsMinMaxClose(window, OnWindowsCollapsed, OnWindowMinMax, OnWindowClose);
        _minMaxClose.SetupColorScheme(initialColorScheme);
        var winbuttons = _minMaxClose.ToPlatform(context);
        rootPanel.Children.Add(winbuttons);
        _mainWindowContent = rootPanel;

        window.Content.PointerMoved += Content_PointerMoved;
        window.Content.PointerPressed += Content_PointerPressed;
        window.Content.PointerReleased += Content_PointerReleased;
        window.Content.PointerCanceled += Content_PointerCanceled;
    }

    private void OnWindowsCollapsed()
    {
        var appwindow = _window.ToAppWindow();
        ((OverlappedPresenter)appwindow.Presenter).Minimize();
    }

    private void OnWindowMinMax()
    {
        var w = (Microsoft.Maui.Controls.Window)_window.GetWindow();
        var appw = _window.ToAppWindow();

        if (!isMaxizedWindow)
        {
            isMaxizedWindow = true;
            lastWindowsLocation = new Rect
            {
                X = w.X, 
                Y = w.Y,
                Height = w.Height,
                Width = w.Width,
            };

            w.X = 0;
            w.Y = 0;
            w.Width = 1920;
            w.Height = 1040;
            _window.SetupCornersStyle(false);
        }
        else
        {
            isMaxizedWindow = false;
            w.X = lastWindowsLocation.X;
            w.Y = lastWindowsLocation.Y;
            w.Width = lastWindowsLocation.Width;
            w.Height = lastWindowsLocation.Height;
            _window.SetupCornersStyle(true);
        }
    }

    private void OnWindowClose()
    {
        _window.Close();
    }

    private void Content_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var ui = (UIElement)sender;
        var par = GetResizeParams(ui, e);
        var stl = par.Cursor;

        if (isMaxizedWindow)
        {
            var mouse = new CoreCursor(CoreCursorType.Arrow, 0);
            MouseCursorWin32.ChangeCursor(ui, InputCursor.CreateFromCoreCursor(mouse));
            return;
        }

        if (pressedMouseState == CoreCursorType.Arrow)
        {
            var mouse = new CoreCursor(stl, 0);
            MouseCursorWin32.ChangeCursor(ui, InputCursor.CreateFromCoreCursor(mouse));
            return;
        }

        var window = Microsoft.Maui.Controls.Application.Current!.Windows.First();
        var mousePos = MouseCursorWin32.GetPositionAsVector2();
        var delta = lastMousePoint - mousePos;
        int dx = (int)delta.X;
        int dy = (int)delta.Y;

        if (dx == 0 && dy == 0)
            return;

        var appWindow = window.ToAppWindow();
        int windowX = appWindow.Position.X;
        int windowY = appWindow.Position.Y;
        int windowWidth = appWindow.Size.Width;
        int windowHeight = appWindow.Size.Height;
        int newX = windowX;
        int newY = windowY;
        int newWidth = windowWidth;
        int newHeight = windowHeight;

        bool useOffsetX = false;
        bool useOffsetY = false;
        switch (resizeDirection)
        {
            case ResizeDirections.Left:
                newWidth += dx;
                newX -= dx;
                break;
            case ResizeDirections.LeftTop:
                newWidth += dx;
                newHeight += dy;
                newX -= dx;
                newY -= dy;
                break;
            case ResizeDirections.Top:
                newHeight += dy;
                newY -= dy;
                break;
            case ResizeDirections.TopRight:
                newWidth -= dx;
                newHeight += dy;
                newY -= dy;
                break;
            case ResizeDirections.Right:
                newWidth -= dx;
                break;
            case ResizeDirections.RightBottom:
                newWidth -= dx;
                newHeight -= dy;
                break;
            case ResizeDirections.Bottom:
                newHeight -= dy;
                break;
            case ResizeDirections.BottomLeft:
                newWidth += dx;
                newHeight -= dy;
                newX -= dx;
                break;
            case ResizeDirections.Move:
                newX = (int)mousePos.X;
                newY = (int)mousePos.Y;
                useOffsetX = true;
                useOffsetY = true;
                break;
            default:
                break;
        }

        if (windowX != newX || windowY != newY)
        {
            int offsetX = useOffsetX ? (int)windowOffset.X : 0;
            int offsetY = useOffsetY ? (int)windowOffset.Y : 0;
            var pos = new global::Windows.Graphics.PointInt32(newX - offsetX, newY - offsetY);
            appWindow.Move(pos);
        }

        if (windowWidth != newWidth || windowHeight != newHeight)
        {
            var size = new global::Windows.Graphics.SizeInt32(newWidth, newHeight);
            appWindow.Resize(size);
        }

        lastMousePoint = mousePos;

        if (pointerMove == null)
        {
            pointerMove = e.Pointer;
            _mainWindowContent.CapturePointer(pointerMove);
        }
    }

    private void Content_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (isMaxizedWindow)
            return;

        var ui = (UIElement)sender;
        var p = e.GetCurrentPoint(ui);
        bool isDrag = false;

        var param = GetResizeParams(ui, e);
        var pressedMouseState = param.Cursor;
        var resizeDirection = param.ResizeDirection;

        if (pressedMouseState != CoreCursorType.Arrow)
        {
            isDrag = true;
        }
        else if (p.Position.Y < 40)
        {
            var point = new System.Drawing.Point((int)p.Position.X, (int)p.Position.Y);
            var window = Microsoft.Maui.Controls.Application.Current!.Windows.First();

            var minmaxclose = _minMaxClose.CalcSize((int)window.Width, (int)window.Height);
            if (minmaxclose.Contains(point))
                return;

            var pointMaui = new Point(p.Position.X, p.Position.Y);
            foreach (var item in _scaffold.UndragArea)
            {
                if (item.Contains(pointMaui))
                    return;
            }

            isDrag = true;
            pressedMouseState = CoreCursorType.Cross;
            resizeDirection = ResizeDirections.Move;
        }

        if (isDrag)
        {
            windowOffset = p.Position;
            this.pressedMouseState = pressedMouseState;
            this.resizeDirection = resizeDirection;
            lastMousePoint = MouseCursorWin32.GetPositionAsVector2();
            pointerClick = e.Pointer;
            _mainWindowContent.CapturePointer(pointerClick);
        }
    }

    private void Content_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        PointerCancel();
    }

    private void Content_PointerCanceled(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        PointerCancel();
    }

    private void PointerCancel()
    {
        pressedMouseState = CoreCursorType.Arrow;
        resizeDirection = ResizeDirections.None;
        if (_mainWindowContent != null)
        {
            if (pointerMove != null)
                _mainWindowContent.ReleasePointerCapture(pointerMove);

            if (pointerClick != null)
                _mainWindowContent.ReleasePointerCapture(pointerClick);

            pointerMove = null;
            pointerClick = null;
        }
    }

    internal void SetupColorScheme(StatusBarColorTypes scheme)
    {
        _minMaxClose.SetupColorScheme(scheme);
    }

    private static ResizeParam GetResizeParams(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        const double min = 10;
        var s = (UIElement)sender;
        var p = e.GetCurrentPoint(s);
        double w = s.RenderSize.Width;
        double h = s.RenderSize.Height;
        double x = p.Position.X;
        double y = p.Position.Y;

        bool isTop = y < min;
        bool isLeft = x < min;
        bool isBottom = y > h - min;
        bool isRight = x > w - min;

        var resizeDirection = ResizeDirections.None;
        var cursorMouseType = CoreCursorType.Arrow;

        if (isTop && isLeft)
        {
            resizeDirection = ResizeDirections.LeftTop;
            cursorMouseType = CoreCursorType.SizeNorthwestSoutheast;
        }
        else if (isTop && isRight)
        {
            resizeDirection = ResizeDirections.TopRight;
            cursorMouseType = CoreCursorType.SizeNortheastSouthwest;
        }
        else if (isBottom && isLeft)
        {
            resizeDirection = ResizeDirections.BottomLeft;
            cursorMouseType = CoreCursorType.SizeNortheastSouthwest;
        }
        else if (isBottom && isRight)
        {
            resizeDirection = ResizeDirections.RightBottom;
            cursorMouseType = CoreCursorType.SizeNorthwestSoutheast;
        }
        else if (isTop || isBottom)
        {
            resizeDirection = isTop ? ResizeDirections.Top : ResizeDirections.Bottom;
            cursorMouseType = CoreCursorType.SizeNorthSouth;
        }
        else if (isRight || isLeft)
        {
            resizeDirection = isRight ? ResizeDirections.Right : ResizeDirections.Left;
            cursorMouseType = CoreCursorType.SizeWestEast;
        }

        return new ResizeParam { Cursor = cursorMouseType, ResizeDirection = resizeDirection };
    }

    private enum ResizeDirections
    {
        None,
        Left,
        LeftTop,
        Top,
        TopRight,
        Right,
        RightBottom,
        Bottom,
        BottomLeft,
        Move,
    }

    private struct ResizeParam
    {
        public CoreCursorType Cursor { get; set; }
        public ResizeDirections ResizeDirection { get; set; }
    }
}