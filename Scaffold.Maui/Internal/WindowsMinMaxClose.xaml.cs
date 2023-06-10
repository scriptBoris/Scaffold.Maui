using ScaffoldLib.Maui.Core;
using Microsoft.Maui.Platform;
using System.Runtime.InteropServices;
using System.Drawing;
using Colors = Microsoft.Maui.Graphics.Colors;
using Color = Microsoft.Maui.Graphics.Color;

#if WINDOWS
using Windows.UI.ViewManagement;
using ScaffoldLib.Maui.Platforms.Windows.Controls;
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace ScaffoldLib.Maui.Internal;

public partial class WindowsMinMaxClose
{
#if WINDOWS
    private bool isMaxed;
    private double lastX;
    private double lastY;
    private double lastWidth;
    private double lastHeight;

    public WindowsMinMaxClose(MauiWinUIWindow window, 
        Action actionButtonCollapse,
        Action actionButtonMinMax,
        Action actionClose)
	{
		InitializeComponent();

        var sizes = new List<Microsoft.Maui.Graphics.Size>()
        {
            buttonCollapse.Measure(500, 500),
            buttonMax.Measure(500, 500),
            buttonClose.Measure(500, 500),
        };
        Area = new System.Drawing.Size((int)sizes.Sum(x => x.Width), (int)sizes.Max(x => x.Height));

        buttonCollapse.TapCommand = new Command(actionButtonCollapse);
        buttonMax.TapCommand = new Command(actionButtonMinMax);
        buttonClose.TapCommand = new Command(actionClose);
	}

    public System.Drawing.Rectangle CalcSize(int windowWidth, int windowHeight)
    {
        var sizes = new List<Microsoft.Maui.Graphics.Size>()
        {
            buttonCollapse.Measure(500, 500),
            buttonMax.Measure(500, 500),
            buttonClose.Measure(500, 500),
        };
        var area = new System.Drawing.Size((int)sizes.Sum(x => x.Width), (int)sizes.Max(x => x.Height));
        return new Rectangle(windowWidth - area.Width, 0, area.Width, area.Height);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (buttonClose == null || buttonMax == null || buttonClose == null)
            return;

        double w = buttonCollapse.Width + buttonMax.Width + buttonClose.Width;
        ClickArea = new Rectangle((int)(width - w), 0, (int)w, 40);
    }

    public Rectangle ClickArea { get; private set; }
    public System.Drawing.Size Area { get; private set; }

    // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
    // Copied from dwmapi.h
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
    // what value of the enum to set.
    // Copied from dwmapi.h
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);
#endif

    public void SetupColorScheme(StatusBarColorTypes scheme)
    {
		Color? color = null;
		switch (scheme)
		{
			case StatusBarColorTypes.Light:
				color = Colors.White;
				break;
			case StatusBarColorTypes.Dark:
				color = Colors.Black;
				break;
			default:
				break;
		}
		imageClose.TintColor = color;
		imageCollapse.TintColor = color;
		imageMax.TintColor = color;
    }
}