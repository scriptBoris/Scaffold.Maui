﻿using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

internal static class Extensions
{
    public static void TryAppearing(this IView iview, bool isComplete = false)
    {
        if (iview is IAppear v)
            v.OnAppear(isComplete);

        if (iview is View view && view.BindingContext is IAppear data && data != iview)
            data.OnAppear(isComplete);
    }

    public static void TryDisappearing(this IView iview, bool isComplete = false)
    {
        if (iview is IDisappear v)
            v.OnDisappear(isComplete);

        if (iview is View view && view.BindingContext is IDisappear data && data != iview)
            data.OnDisappear(isComplete);
    }

    public static void TryRemoveFromNavigation(this IView iview)
    {
        if (iview is IRemovedFromNavigation rm)
            rm.OnRemovedFromNavigation();
    }

    public static void TryAppearing(this IAgent agent, bool isComplete, AppearingStates parentStl, Color? navigationBarBgColor = null)
    {
        if (parentStl == AppearingStates.Disappear)
            return;

        if (isComplete == false)
        {
            agent.IsAppear = true;

            var statusBarStyle = Scaffold.GetStatusBarForegroundColor(agent.ViewWrapper.View);
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

        if (agent is IAppear ap)
            ap.OnAppear(isComplete);

        agent.ViewWrapper.View.TryAppearing(isComplete);
    }

    public static void TryDisappearing(this IAgent agent, bool isComplete, AppearingStates parentStl)
    {
        if (parentStl == AppearingStates.Disappear)
            return;

        if (isComplete == false)
            agent.IsAppear = false;

        if (agent is IDisappear dis)
            dis.OnDisappear(isComplete);

        agent.ViewWrapper.View.TryDisappearing(isComplete);
    }

    public static void TryRemoveFromNavigation(this IAgent agent)
    {
        agent.ViewWrapper.View.TryRemoveFromNavigation();
        if (agent is IRemovedFromNavigation rm)
            rm.OnRemovedFromNavigation();
    }

    public static void ResolveStatusBarColor(this IAgent frame)
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
            var bgColor = vbar ?? rbar ?? Scaffold.DefaultNavigationBarBackgroundColor;
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

    public static Task AwaitReady(this IAgent agent, CancellationToken? cancellationToken = null)
    {
        var view = (View)agent.ViewWrapper.View;
        return AwaitReady(view, cancellationToken);
    }

    public static Task AwaitReady(this View view, CancellationToken? cancellation = null)
    {
        var cancel = cancellation ?? CancellationToken.None;
#if ANDROID
        return AwaitReadyDroid(view, cancel);
#elif IOS
        return AwaitReadyIOS(view, cancel);
#else
        return Task.CompletedTask;
#endif
    }

#if IOS
    private static async Task AwaitReadyIOS(View view, CancellationToken cancellation)
    {
        var handler = await view.AwaitHandler(cancellation);
        if (handler == null)
            return;


        var tsc = new TaskCompletionSource();
        view.Animate("checkisload",
            callback: (x) => { },
            rate: 1,
            //length: 10,
            length: 1,
            easing: null,
            finished: (x, isFail) =>
            {
                //await Task.Delay(25);
                tsc.SetResult();
            }
        );

        //var uiview = view.Handler?.PlatformView as UIKit.UIView;
        //var animator = new UIKit.UIViewPropertyAnimator(20, 16f, () => { });
        //animator.AddCompletion((pos) =>
        //{
        //    tsc.SetResult(true);
        //});

        await tsc.Task.WithCancelation(cancellation);
    }
#endif

#if ANDROID
    private static async Task AwaitReadyDroid(View view, CancellationToken cancel)
    {
        var readyLayout = new TaskCompletionSource<bool>();
        var readyDraw = new TaskCompletionSource<bool>();
        var readyGlobalDraw = new TaskCompletionSource<bool>();
        var h = await AwaitHandler(view, cancel);
        if (h == null)
            return;

        Android.Views.View av = (Android.Views.View)h.PlatformView!;
        Android.Views.ViewTreeObserver? vto = null;

        try
        {
            vto = av?.ViewTreeObserver;
        }
        catch (Exception ex)
        {
            return;
        }

        if (vto == null)
            return;

        var time = DateTime.Now;

        void Change(object? o, Android.Views.View.LayoutChangeEventArgs e)
        {
            if (av.MeasuredHeight > 0 || av.MeasuredWidth > 0)
            {
                readyLayout.TrySetResult(true);
            }
        }
        void Draw(object? o, EventArgs e)
        {
            readyDraw.TrySetResult(true);
        }
        void GlobalDraw(object? o, EventArgs e)
        {
            readyGlobalDraw.TrySetResult(true);
        }

        vto.PreDraw += Draw;
        vto.GlobalLayout += GlobalDraw;
        av.LayoutChange += Change;

        await Task.WhenAll(readyLayout.Task, readyDraw.Task, readyGlobalDraw.Task).WithCancelation(cancel);

        if (vto.IsAlive)
        {
            vto.PreDraw -= Draw;
            vto.GlobalLayout -= GlobalDraw;
        }
        av.LayoutChange -= Change;

        if (cancel.IsCancellationRequested)
        {
            readyLayout.TrySetCanceled();
            readyDraw.TrySetCanceled();
            readyGlobalDraw.TrySetCanceled();
        }

        var lat = DateTime.Now - time;
        if (lat > TimeSpan.FromMilliseconds(45))
        {
            if (view is IHardView hard)
                await hard.ReadyToPush.WithCancelation(cancel);
            else
                await Task.Delay(250).WithCancelation(cancel);
        }
    }
#endif

    public static async Task<IViewHandler?> AwaitHandler(this View view, CancellationToken? cancel = null)
    {
        if (view.Handler != null)
            return view.Handler;

        var tsc = new TaskCompletionSource<IViewHandler>();
        void eventDelegate(object? sender, EventArgs e)
        {
            tsc.TrySetResult(view.Handler!);
        }

        view.HandlerChanged += eventDelegate;
        var handler = await tsc.Task.WithCancelation(cancel ?? CancellationToken.None);
        view.HandlerChanged -= eventDelegate;

        return handler;
    }

    public static bool IsDark(this Color col)
    {
        double Y = 0.299 * col.Red + 0.587 * col.Green + 0.114 * col.Blue;
        if (Y > 0.5d)
            return false;
        else
            return true;
    }

    public static Scaffold? GetRootScaffold(this Microsoft.Maui.Controls.Page page)
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

    internal static INotifyCollectionChanged AsNotifyObs<T>(this ReadOnlyObservableCollection<T> obs)
    {
        return obs;
    }

    internal static Rect AbsRect(this View view)
    {
        var rect = view.Frame;
        var parent = view.Parent as View;
        while (parent != null)
        {
            rect = rect.Offset(parent.X, parent.Y);
            parent = parent.Parent as View;
        }
        return rect;
    }

    internal static IEnumerable<View> GetDeepAllChildren(this View view)
    {
        var list = new List<View>();
        switch (view)
        {
            case Layout l:
                foreach (View item in l.Children)
                {
                    list.Add(item);
                    list.AddRange(item.GetDeepAllChildren());
                }
                break;
            case ContentView c:
                list.Add(c.Content);
                list.AddRange(c.Content.GetDeepAllChildren());
                break;
            default:
                break;
        }
        return list;
    }

    internal static async Task<T?> WithCancelation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        try
        {
            return await task.WaitAsync(cancellationToken);
        }
        catch (Exception)
        {
            return default;
        }
    }

    internal static async Task WithCancelation(this Task task, CancellationToken cancellationToken)
    {
        try
        {
            await task.WaitAsync(cancellationToken);
        }
        catch (Exception)
        {
        }
    }

    internal static Size WithPadding(this Size size, Thickness padding)
    {
        return new Size(size.Width + padding.HorizontalThickness, size.Height + padding.VerticalThickness);
    }

    internal static void InvalidateMeasureHardcore(this View view)
    {
        ((IView)view).InvalidateMeasure();
    }

    internal static T? GetTargetOrDefault<T>(this WeakReference<T> self)
        where T : class
    {
        if (self == null)
            throw new ArgumentNullException(nameof(self));

        if (self.TryGetTarget(out var target))
            return target;

        return default;
    }
}