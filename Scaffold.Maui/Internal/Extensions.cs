using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal
{
    internal static class Extensions
    {
        public static void TryAppearing(this IView iview, bool isComplete = false)
        {
            if (iview is IAppear v)
                v.OnAppear(isComplete);
        }

        public static void TryDisappearing(this IView iview, bool isComplete = false)
        {
            if (iview is IDisappear v)
                v.OnDisappear(isComplete);
        }

        public static void TryRemoveFromNavigation(this IView iview)
        {
            if (iview is IRemovedFromNavigation rm)
                rm.OnRemovedFromNavigation();
        }

        public static void TryAppearing(this IFrame frame, bool isComplete, AppearingStates parentStl, Color? navigationBarBgColor = null)
        {
            if (parentStl == AppearingStates.Disappear)
                return;

            if (isComplete == false)
            {
                frame.IsAppear = true;

                var statusBarStyle = Scaffold.GetStatusBarForegroundColor(frame.ViewWrapper.View);
                if (statusBarStyle != StatusBarColorTypes.DependsByNavigationBarColor)
                {
                    Scaffold.SetupStatusBarColor(statusBarStyle);
                }
                else if (navigationBarBgColor != null)
                {
                    if (navigationBarBgColor.IsDark())
                        Scaffold.SetupStatusBarColor(StatusBarColorTypes.Light);
                    else
                        Scaffold.SetupStatusBarColor(StatusBarColorTypes.Dark);
                }
            }

            frame.ViewWrapper.View.TryAppearing(isComplete);
            if (frame is IAppear ap)
                ap.OnAppear(isComplete);
        }

        public static void TryDisappearing(this IFrame frame, bool isComplete, AppearingStates parentStl)
        {
            if (parentStl == AppearingStates.Disappear)
                return;

            if (isComplete == false)
                frame.IsAppear = false;

            frame.ViewWrapper.View.TryDisappearing(isComplete);
            if (frame is IDisappear dis)
                dis.OnDisappear(isComplete);
        }

        public static void TryRemoveFromNavigation(this IFrame frame)
        {
            frame.ViewWrapper.View.TryRemoveFromNavigation();
            if (frame is IRemovedFromNavigation rm)
                rm.OnRemovedFromNavigation();
        }

        public static void ResolveStatusBarColor(this IFrame frame)
        {
            if (!frame.IsAppear)
                return;

            var type = Scaffold.GetStatusBarForegroundColor(frame.ViewWrapper.View);
            if (type != StatusBarColorTypes.DependsByNavigationBarColor)
            {
                Scaffold.SetupStatusBarColor(type);
            }
            else
            {
                var rbar = (frame.NavigationBar as View)?.BackgroundColor;
                var vbar = Scaffold.GetNavigationBarBackgroundColor(frame.ViewWrapper.View);
                var bgColor = vbar ?? rbar ?? Scaffold.defaultNavigationBarBackgroundColor;
                if (bgColor.IsDark())
                    Scaffold.SetupStatusBarColor(StatusBarColorTypes.Light);
                else
                    Scaffold.SetupStatusBarColor(StatusBarColorTypes.Dark);
            }
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
            anim.Commit(v, "Anim", length: length, finished: (v, b) =>
            {
                tsc.TrySetResult(b);
            });
            return tsc.Task;
        }

        public static byte[] ToBytes(this Stream str)
        {
            byte[] byteArray;
            str.Position = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                str.CopyTo(ms);
                byteArray = ms.ToArray();
            }
            str.Position = 0;
            return byteArray;
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

            //if (view.IsLoaded)
            //{
            //    return handler;
            //}
            //else
            //{
            //    var tscLoaded = new TaskCompletionSource<bool>();
            //    void eventLoaded(object? sender, EventArgs e)
            //    {
            //        tscLoaded.TrySetResult(true);
            //    }

            //    view.Loaded += eventLoaded;
            //    await tscLoaded.Task;
            //    view.Loaded -= eventLoaded;
            //}

            return handler;
        }

        public static bool IsDark(this Color col)
        {
            double Y = 0.299 * col.Red + 0.587 * col.Green + 0.114 * col.Blue;
            //var I = 0.596 * col.Red - 0.274 * col.Green - 0.322 * col.Blue;
            //var Q = 0.211 * col.Red - 0.523 * col.Green + 0.312 * col.Blue;
            //if (Y > 0.5d)
            if (Y > 0.5d)
                return false;
            else
                return true;
        }

        public static Scaffold? GetRootScaffold(this Page page)
        {
            if (page is ContentPage c)
            {
                //if (c.Content is Scaffold cv)
                //    return cv;
                //else
                    return FindScaffold(c.Content) as Scaffold;
            }
            return null;
        }

        private static View? FindScaffold(View view)
        {
            switch (view)
            {
                case Scaffold vc:
                    return vc;

                case ContentView cv:
                    return FindScaffold(cv);

                case Layout l:
                    foreach (var item in l.Children)
                        return FindScaffold((View)item);
                    break;

                default:
                    return null;
            }

            return null;
        }
    }
}
