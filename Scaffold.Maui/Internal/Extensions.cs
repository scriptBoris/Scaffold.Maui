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
        public static void TryAppearing(this IView iview, bool isComplete = false)
        {
            if (iview is IAppear v)
                v.OnAppear(isComplete);

            // avoid stackoverflow (vm != view)
            //if (iview is View mauiView && mauiView.BindingContext is IAppear vm && vm != iview)
            //    vm.OnAppear(isComplete);
        }

        public static void TryDisappearing(this IView iview, bool isComplete = false)
        {
            if (iview is IDisappear v)
                v.OnDisappear(isComplete);

            // avoid stackoverflow (vm != view)
            //if (iview is View mauiView && mauiView.BindingContext is IDisappear vm && vm != iview)
            //    vm.OnDisappear(isComplete);
        }

        public static void TryAppearing(this IFrame frame, bool isComplete = false)
        {
            frame.ViewWrapper.View.TryAppearing(isComplete);
        }

        public static void TryDisappearing(this IFrame frame, bool isComplete = false)
        {
            frame.ViewWrapper.View.TryDisappearing(isComplete);
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
