using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using AView = Android.Views.View;
#endif

namespace ScaffoldLib.Maui.Toolkit;

public static class Extensions
{
    public static string GetDisplayItemText(this object self, string? displayProperty)
    {
        object? currentObject = self;
        if (!string.IsNullOrWhiteSpace(displayProperty))
        {
            string[] propertyNames = displayProperty.Split('.');

            foreach (string propertyName in propertyNames)
            {
                var propertyInfo = currentObject.GetType().GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    currentObject = null;
                    break;
                }

                currentObject = propertyInfo.GetValue(currentObject);
                if (currentObject == null)
                {
                    currentObject = null;
                    break;
                }
            }

            if (currentObject == null)
                currentObject = self;
        }

        return currentObject.ToString() ?? self.GetType().Name;
    }

    public static Point? GetPositionOnDisplay(this View view)
    {
#if ANDROID
        int[] location = new int[2];
        var native = view.Handler?.PlatformView as AView;
        if (native == null)
            return null;

        native.GetLocationOnScreen(location);
        int x = location[0];
        int y = location[1];
        double den = Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo.Density;
        double xres = x / den;
        double yres = y / den;
        return new Point(xres, yres);
#else
        throw new NotImplementedException();
#endif
    }

    /// <summary>
    /// Extended default MAUI animation method, but cancellation support
    /// </summary>
    /// <param name="view"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="name"></param>
    /// <param name="updateAction"></param>
    /// <param name="length"></param>
    /// <param name="easing"></param>
    /// <param name="cancel"></param>
    /// <returns>True - animation finished; False - canceled or break</returns>
    public static async Task<bool> AnimateTo(this VisualElement view,
        double start,
        double end,
        string name,
        Action<VisualElement, double> updateAction,
        uint length = 250,
        Easing? easing = null,
        CancellationToken? cancel = null
    )
    {
        // Если отмена уже сработала
        if (cancel?.IsCancellationRequested == true)
            return false;
        //return Task.FromResult(false);

        easing ??= Easing.Linear;
        var tcs = new TaskCompletionSource<bool>();
        var weakView = new WeakReference<VisualElement>(view);

        void UpdateProperty(double f)
        {
            if (weakView.TryGetTarget(out VisualElement? v))
            {
                updateAction(v, f);
            }
        }

        var animation = new Animation(UpdateProperty, start, end, easing);
        CancellationTokenRegistration? registration = null;

        if (cancel != null)
        {
            registration = cancel.Value.Register(() =>
            {
                animation.Dispose();
                tcs.TrySetResult(false);
                //if (!animation.IsDisposed && animation.IsEnabled)
                //{
                //    animation.Dispose();
                //    tcs.TrySetResult(false);
                //}
            });
        }

        animation.Commit(
            view,
            name,
            rate: 16,
            length: length,
            finished: (f, isCanceled) =>
            {
                tcs.TrySetResult(!isCanceled);
            });

        var result = await tcs.Task;
        registration?.Dispose();
        return result;
    }
}
