using Scaffold.Maui.Containers;
using Scaffold.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Internal
{
    internal static class Extensions
    {
        public static void TryAppearing(this object obj, bool isComplete = false)
        {
            if (obj is IAppear ap)
                ap.OnAppear(isComplete);
            else if (obj is View view && view.BindingContext is IAppear ap2)
                ap2.OnAppear(isComplete);
        }

        public static void TryDisappearing(this object obj, bool isComplete = false)
        {
            if (obj is IDisappear ap)
                ap.OnDisappear(isComplete);
            else if (obj is View view && view.BindingContext is IDisappear ap2)
                ap2.OnDisappear(isComplete);
        }

        public static void TryAppearing(this IFrame frame, bool isComplete = false)
        {
            var view = frame.ViewWrapper.View;
            if (view is IAppear ap)
                ap.OnAppear(isComplete);
            else if (view.BindingContext is IAppear ap2)
                ap2.OnAppear(isComplete);
        }

        public static void TryDisappearing(this IFrame frame, bool isComplete = false)
        {
            var view = frame.ViewWrapper.View;
            if (view is IDisappear ap)
                ap.OnDisappear(isComplete);
            else if (view.BindingContext is IDisappear ap2)
                ap2.OnDisappear(isComplete);
        }

        public static T? ItemOrDefault<T>(this IList<T> self, int index)
        {
            if (self.Count == 0)
                return default;

            if (index < 0 || index > self.Count - 1)
                return default;

            return self[index];
        }

        public static Task<bool> EasyAnimate(this View v, string name, double start, double end, ushort length, Action<double> callback)
        {
            var tsc = new TaskCompletionSource<bool>();
            var anim = new Animation(callback, start, end);
            anim.Commit(v, "Anim", length:length, finished: (v, b) =>
            {
                tsc.TrySetResult(b);
            });
            return tsc.Task;
        }

        public static async Task<IViewHandler> AwaitHandler(this View view)
        {
            if (view.Handler != null)
                return view.Handler;

            var tsc = new TaskCompletionSource<IViewHandler>();
            void eventDelegate(object? sender, EventArgs e)
            {
                tsc.TrySetResult(view.Handler!);
            }

            view.HandlerChanged += eventDelegate;
            var handler = await tsc.Task;
            view.HandlerChanged -= eventDelegate;

//#if ANDROID
//            if (handler.PlatformView is Android.Views.View aview)
//            {
//                var tsc2 = new TaskCompletionSource<bool>();

//                if (aview.IsAttachedToWindow)
//                {

//                }

//                void argEnd(object? sender, Android.Views.View.ViewAttachedToWindowEventArgs args)
//                {
//                    tsc2.TrySetResult(true);
//                }
//                aview.ViewAttachedToWindow += argEnd;
//                await tsc2.Task;
//            }
//#endif
            return handler;
        }
    }
}
