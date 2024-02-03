using ButtonSam.Maui.Core;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal;

internal class NavigationController : IDisposable
{
    private readonly Scaffold _scaffold;
    private readonly ObservableCollection<IAgent> _agents = new();
    private readonly ObservableCollection<View> _navigationStack = new();

    public NavigationController(Scaffold scaffold)
    {
        _scaffold = scaffold;
        NavigationStack = new(_navigationStack);
        Agents = new (_agents);
    }

    private CancellationTokenSource cancelAnim = new();
    private AppearingStates AppearingStl => _scaffold.AppearingState;
    public ReadOnlyObservableCollection<View> NavigationStack { get; private set; }
    public ReadOnlyObservableCollection<IAgent> Agents { get; private set; }
    public IAgent? CurrentAgent => Agents.LastOrDefault();

    internal async Task PushAsync(View view, bool isAnimated, Agent? currentAgent = null, NavigatingTypes? intentType = null)
    {
        var oldAgent = currentAgent ?? CurrentAgent;
        if (oldAgent == null)
            isAnimated = false;

        var bgColor = Scaffold.GetNavigationBarBackgroundColor(view) ?? _scaffold.NavigationBarBackgroundColor ?? Scaffold.DefaultNavigationBarBackgroundColor;
        var fgColor = Scaffold.GetNavigationBarForegroundColor(view) ?? _scaffold.NavigationBarForegroundColor ?? Scaffold.DefaultNavigationBarForegroundColor;

        var args = new CreateAgentArgs
        {
            Context = _scaffold,
            View = view,
            NavigationBarBackgroundColor = bgColor,
            NavigationBarForegroundColor = fgColor,
            IndexInStack = NavigationStack.Count,
            SafeArea = _scaffold.SafeArea,
            BackButtonBehavior = _scaffold.BackButtonBehavior,
            Behaviors = _scaffold.ExternalBevahiors.ToArray(),
        };

        // Batch start
        _scaffold.BatchBegin();

        var newAgent = _scaffold.ViewFactory.CreateAgent(args);
        _agents.Add(newAgent);
        _navigationStack.Add(view);
        _scaffold.Children.Insert(_scaffold.Children.Count - 1, (View)newAgent);

        Scaffold.TryHideKeyboard();
        oldAgent?.TryDisappearing(false, AppearingStl);
        newAgent.TryAppearing(false, AppearingStl, bgColor);

        if (!isAnimated)
        {
            // Batch end
            _scaffold.BatchCommit();
        }
        else
        {
            cancelAnim.Cancel();
            cancelAnim = new();
            var cancel = cancelAnim.Token;

            oldAgent?.PrepareAnimate(NavigatingTypes.UnderPush);
            newAgent.PrepareAnimate(NavigatingTypes.Push);

            // Batch end
            _scaffold.BatchCommit();

            await newAgent.AwaitReady(cancel);
            var anim = newAgent.GetAnimation(NavigatingTypes.Push);
            if (anim.UsingPlatformAnimation)
            {
                var taskOld = oldAgent?.DoPlatformAnimation(NavigatingTypes.UnderPush) ?? Task.CompletedTask;
                var taskNew = newAgent.DoPlatformAnimation(NavigatingTypes.Push);
                await Task.WhenAll(taskOld, taskNew);
            }
            else
            {
                await _scaffold.TransitAnimation("push", 0, 1, anim.Time, anim.Easing, x =>
                {
                    _scaffold.BatchBegin();
                    oldAgent?.DoAnimation(x, NavigatingTypes.UnderPush);
                    newAgent.DoAnimation(x, NavigatingTypes.Push);
                    _scaffold.BatchCommit();
                });
            }
            //#if IOS
            //            var tscPush = new TaskCompletionSource();
            //            if (newAgent is View va && va.Handler.PlatformView is UIKit.UIView uiv)
            //            {
            //                var animator = new UIKit.UIViewPropertyAnimator(0.200, UIKit.UIViewAnimationCurve.EaseOut,
            //                    () =>
            //                    {
            //                        oldAgent?.DoAnimation(1, NavigatingTypes.UnderPush);
            //                        newAgent.DoAnimation(1, NavigatingTypes.Push);
            //                    });
            //                animator.StartAnimation();
            //                animator.AddCompletion(pos =>
            //                {
            //                    tscPush.TrySetResult();
            //                });
            //            }
            //            await tscPush.Task;

            //#else
            //await _scaffold.TransitAnimation("push", 0, 1, anim.Time, anim.Easing, x =>
            //{
            //    _scaffold.BatchBegin();
            //    oldAgent?.DoAnimation(x, NavigatingTypes.UnderPush);
            //    newAgent.DoAnimation(x, NavigatingTypes.Push);
            //    _scaffold.BatchCommit();
            //});
            //#endif

            if (cancel.IsCancellationRequested)
                return;
        }

        if (oldAgent is View oldAgentView)
            oldAgentView.IsVisible = false;

        oldAgent?.TryDisappearing(true, AppearingStl);
        newAgent.TryAppearing(true, AppearingStl);
    }

    internal void InsertView(View view, int index)
    {
        var bgColor = Scaffold.GetNavigationBarBackgroundColor(view) ?? _scaffold.NavigationBarBackgroundColor ?? Scaffold.DefaultNavigationBarBackgroundColor;
        var fgColor = Scaffold.GetNavigationBarForegroundColor(view) ?? _scaffold.NavigationBarForegroundColor ?? Scaffold.DefaultNavigationBarForegroundColor;

        var args = new CreateAgentArgs
        {
            Context = _scaffold,
            IndexInStack = index,
            View = view,
            NavigationBarBackgroundColor = bgColor,
            NavigationBarForegroundColor = fgColor,
            SafeArea = _scaffold.SafeArea,
            BackButtonBehavior = _scaffold.BackButtonBehavior,
            Behaviors = _scaffold.ExternalBevahiors.ToArray(),
        };
        var newAgent = _scaffold.ViewFactory.CreateAgent(args);
        var newAgentView = (View)newAgent;
        newAgentView.IsVisible = false;

        _navigationStack.Insert(index, view);
        _agents.Insert(index, newAgent);
        _scaffold.Children.Insert(index, newAgentView);
    }

    internal async Task ReplaceView(View view, bool isAnimated)
    {
        var oldAgent = CurrentAgent!;
        var oldView = oldAgent.ViewWrapper.View;
        var bgColor = Scaffold.GetNavigationBarBackgroundColor(view) ?? _scaffold.NavigationBarBackgroundColor ?? Scaffold.DefaultNavigationBarBackgroundColor;
        var fgColor = Scaffold.GetNavigationBarForegroundColor(view) ?? _scaffold.NavigationBarForegroundColor ?? Scaffold.DefaultNavigationBarForegroundColor;

        var args = new CreateAgentArgs
        {
            Context = _scaffold,
            View = view,
            NavigationBarBackgroundColor = bgColor,
            NavigationBarForegroundColor = fgColor,
            IndexInStack = NavigationStack.Count,
            SafeArea = _scaffold.SafeArea,
            BackButtonBehavior = _scaffold.BackButtonBehavior,
            Behaviors = _scaffold.ExternalBevahiors.ToArray(),
        };

        // Batch start
        _scaffold.BatchBegin();

        var newAgent = _scaffold.ViewFactory.CreateAgent(args);
        _agents.Add(newAgent);
        _navigationStack.Add(view);
        _scaffold.Children.Insert(_scaffold.Children.Count - 1, (View)newAgent);
        _navigationStack.Remove(oldView);
        _agents.Remove(oldAgent);

        Scaffold.TryHideKeyboard();
        oldAgent.TryDisappearing(false, AppearingStl);
        newAgent.TryAppearing(false, AppearingStl, bgColor);

        if (isAnimated)
        {
            cancelAnim.Cancel();
            cancelAnim = new();
            var cancel = cancelAnim.Token;

            oldAgent.PrepareAnimate(NavigatingTypes.UnderReplace);
            newAgent.PrepareAnimate(NavigatingTypes.Replace);

            // Batch end
            _scaffold.BatchCommit();

            await newAgent.AwaitReady(cancel);

            var anim = newAgent.GetAnimation(NavigatingTypes.Replace);
            await _scaffold.TransitAnimation("replace", 0, 1, anim.Time, anim.Easing, x =>
            {
                _scaffold.BatchBegin();
                oldAgent.DoAnimation(x, NavigatingTypes.UnderReplace);
                newAgent.DoAnimation(x, NavigatingTypes.Replace);
                _scaffold.BatchCommit();
            });

            if (cancel.IsCancellationRequested)
                return;
        }

        _scaffold.Children.Remove((View)oldAgent);

        oldAgent.TryDisappearing(true, AppearingStl);
        newAgent.TryAppearing(true, AppearingStl);
        oldAgent.Dispose();

        if (!isAnimated)
        {
            // Batch end
            _scaffold.BatchCommit();
        }
    }

    internal async Task<bool> PopAsync(bool isAnimated)
    {
        var currentAgent = CurrentAgent;
        if (currentAgent == null)
            return false;

        int count = NavigationStack.Count;
        var previosAgent = Agents.ItemOrDefault(count - 2);
        if (previosAgent == null)
            return false;

        if (previosAgent is View prevView)
            prevView.IsVisible = true;

        _navigationStack.Remove(currentAgent.ViewWrapper.View);
        _agents.Remove(currentAgent);

        Scaffold.TryHideKeyboard();
        currentAgent.TryDisappearing(false, AppearingStl);
        previosAgent.TryAppearing(false, AppearingStl, previosAgent.NavigationBarBackgroundColor);

        if (isAnimated)
        {
            cancelAnim.Cancel();
            cancelAnim = new();
            previosAgent.PrepareAnimate(NavigatingTypes.UnderPop);
            currentAgent.PrepareAnimate(NavigatingTypes.Pop);

            var anim = currentAgent.GetAnimation(NavigatingTypes.Pop);
            await _scaffold.TransitAnimation("pop", 0, 1, anim.Time, anim.Easing, x =>
            {
                _scaffold.BatchBegin();
                previosAgent.DoAnimation(x, NavigatingTypes.UnderPop);
                currentAgent.DoAnimation(x, NavigatingTypes.Pop);
                _scaffold.BatchCommit();
            });
        }

        _scaffold.Children.Remove((View)currentAgent);
        currentAgent.TryDisappearing(true, AppearingStl);
        previosAgent.TryAppearing(true, AppearingStl);
        previosAgent.RestoreVisualState();
        currentAgent.TryRemoveFromNavigation();
        currentAgent.Dispose();

        return true;
    }

    internal bool RemoveView(int index)
    {
        if (index < 0 || index >= NavigationStack.Count)
            return false;

        bool isIntantPop = index > 0 && index == NavigationStack.Count - 1;
        var agent = Agents[index];

        if (isIntantPop)
            agent.TryDisappearing(false, AppearingStl);

        _navigationStack.RemoveAt(index);
        _agents.RemoveAt(index);
        _scaffold.Children.Remove((View)agent);

        if (isIntantPop)
            agent.TryDisappearing(true, AppearingStl);

        agent.TryRemoveFromNavigation();
        agent.Dispose();

        if (isIntantPop)
        {
            var under = Agents[index - 1];
            under.TryAppearing(false, AppearingStl);
            under.TryAppearing(true, AppearingStl);
            under.RestoreVisualState();
        }

        return true;
    }

    public void Dispose()
    {
        foreach (var agent in _agents.Reverse())
        {
            agent.Dispose();
        }
    }
}
