using ScaffoldLib.Maui.Containers;
using ScaffoldLib.Maui.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Internal
{
    internal class NavigationController : IDisposable
    {
        private readonly Scaffold _scaffold;
        private readonly ObservableCollection<IFrame> _frames = new();
        private readonly ObservableCollection<View> _navigationStack = new();

        public NavigationController(Scaffold layout)
        {
            _scaffold = layout;
            NavigationStack = new(_navigationStack);
            Frames = new (_frames);
        }

        public ReadOnlyObservableCollection<View> NavigationStack { get; private set; }
        public ReadOnlyObservableCollection<IFrame> Frames { get; private set; }
        public IFrame? CurrentFrame => Frames.LastOrDefault();

        internal async Task<IFrame> PushAsync(View view, bool isAnimated, IFrame? currentFrame = null, NavigatingTypes? intentType = null)
        {
            var oldFrame = currentFrame ?? CurrentFrame;
            if (oldFrame == null)
                isAnimated = false;

            var frame = _scaffold.ViewFactory.CreateFrame(view);
            frame.NavigationBar?.UpdateBackButtonBehavior(_scaffold.BackButtonBehavior);

            _frames.Add(frame);
            _navigationStack.Add(view);
            _scaffold.Children.Insert(_scaffold.Children.Count - 1, (View)frame);

            TryHideKeyboard();
            oldFrame?.TryDisappearing();
            frame.TryAppearing();

            await frame.UpdateVisual(new NavigatingArgs
            {
                NavigationType = intentType ?? NavigatingTypes.Push,
                NewContent = view,
                OldContent = oldFrame?.ViewWrapper.View,
                IsAnimating = isAnimated,
                HasBackButton = NavigationStack.Count > 1,
                SafeArea = _scaffold.SafeArea,
                NavigationBarBackgroundColor = Scaffold.GetNavigationBarBackgroundColor(view),
                NavigationBarForegroundColor = Scaffold.GetNavigationBarForegroundColor(view),
            });

            if (oldFrame is View oldFrameView)
                oldFrameView.IsVisible = false;

            oldFrame?.TryDisappearing(true);
            frame.TryAppearing(true);

            return frame;
        }

        internal async Task<bool> PopAsync(bool isAnimated)
        {
            var currentFrame = CurrentFrame;
            if (currentFrame == null)
                return false;

            int count = NavigationStack.Count;
            bool hasBackButton = count > 2;
            var prevFrame = Frames.ItemOrDefault(count - 2);
            if (prevFrame == null)
                return false;

            if (prevFrame is View prevFrameView)
                prevFrameView.IsVisible = true;

            _navigationStack.Remove(currentFrame.ViewWrapper.View);
            _frames.Remove(currentFrame);

            TryHideKeyboard();
            currentFrame.TryDisappearing();
            prevFrame.TryAppearing();

            await currentFrame.UpdateVisual(new NavigatingArgs
            {
                NavigationType = NavigatingTypes.Pop,
                NewContent = prevFrame.ViewWrapper.View,
                OldContent = currentFrame.ViewWrapper.View,
                IsAnimating = isAnimated,
                HasBackButton = hasBackButton,
                SafeArea = _scaffold.SafeArea,
                NavigationBarBackgroundColor = Scaffold.GetNavigationBarBackgroundColor(prevFrame.ViewWrapper.View),
                NavigationBarForegroundColor = Scaffold.GetNavigationBarForegroundColor(prevFrame.ViewWrapper.View),
            });

            _scaffold.Children.Remove((View)currentFrame);

            currentFrame.TryDisappearing(true);
            prevFrame.TryAppearing(true);

            return true;
        }

        internal async Task<bool> ReplaceAsync(View oldView, View newView, bool isAnimated)
        {
            var oldIndex = NavigationStack.IndexOf(oldView);
            if (oldIndex < 0)
                return false;

            TryHideKeyboard();

            var oldFrame = Frames[oldIndex];
            _frames.RemoveAt(oldIndex);
            _navigationStack.RemoveAt(oldIndex);
            oldFrame.TryDisappearing();

            await PushAsync(newView, isAnimated, oldFrame, NavigatingTypes.Replace);
            _scaffold.Children.Remove((View)oldFrame);
            oldFrame.TryDisappearing(true);

            return true;
        }

        internal bool RemoveView(int index)
        {
            if (index < 0 || index >= NavigationStack.Count)
                return false;

            var frame = Frames[index];
            _navigationStack.RemoveAt(index);
            _frames.RemoveAt(index);
            _scaffold.Children.Remove((View)frame);
            return true;
        }

        internal async Task<bool> InsertView(View view, int index, bool isAnimated)
        {
            if (index < 0)
                return false;

            int count = NavigationStack.Count;
            if (count == 0 || index >= count)
            {
                await PushAsync(view, isAnimated);
                return true;
            }
            else
            {
                var frame = _scaffold.ViewFactory.CreateFrame(view);
                frame.NavigationBar?.UpdateBackButtonBehavior(_scaffold.BackButtonBehavior);

                if (frame is View v)
                    v.IsVisible = false;

                _navigationStack.Insert(index, view);
                _frames.Insert(index, frame);
                _scaffold.Children.Insert(index, (View)frame);

                return true;
            }
        }

        private void TryHideKeyboard()
        {
#if ANDROID
            var context = Platform.AppContext;
            if (context.GetSystemService(Android.Content.Context.InputMethodService) is Android.Views.InputMethods.InputMethodManager inputMethodManager)
            {
                var activity = Platform.CurrentActivity;
                var token = activity?.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, Android.Views.InputMethods.HideSoftInputFlags.None);
                activity?.Window?.DecorView.ClearFocus();
            }
#endif
        }

        public void Dispose()
        {
            foreach (var frame in _frames.Reverse())
            {
                if (frame.ViewWrapper.View is IRemovedFromNavigation v)
                    v.OnRemovedFromNavigation();

                if (frame is IDisposable disposable) 
                    disposable.Dispose();
            }
        }
    }
}
