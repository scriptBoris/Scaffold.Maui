using Scaffold.Maui.Containers;
using Scaffold.Maui.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Internal
{
    internal class NavigationController
    {
        private readonly ScaffoldView _scaffold;
        private readonly List<IFrame> _frames = new();
        private readonly ObservableCollection<View> _navigationStack = new();

        public NavigationController(ScaffoldView layout)
        {
            _scaffold = layout;
            NavigationStack = new(_navigationStack);
            Frames = new ReadOnlyCollection<IFrame>(_frames);
        }

        public ReadOnlyObservableCollection<View> NavigationStack { get; private set; }
        public ReadOnlyCollection<IFrame> Frames { get; private set; }
        public IFrame? CurrentFrame => Frames.LastOrDefault();

        internal async Task<IFrame> PushAsync(View view, bool isAnimated, IFrame? currentFrame = null, NavigatingTypes? intentType = null)
        {
            var oldFrame = currentFrame ?? CurrentFrame;
            if (oldFrame == null)
                isAnimated = false;

            var frame = _scaffold.ViewFactory.CreateFrame(view);
            frame.NavigationBar?.UpdateBackButtonBehavior(_scaffold.BackButtonBehavior);

            _frames.Add(frame);
            _scaffold.Children.Insert(_scaffold.Children.Count - 1, (View)frame);
            _navigationStack.Add(view);

            TryHideKeyboard();
            oldFrame?.TryDisappearing();
            view.TryAppearing();

            await frame.UpdateVisual(new NavigatingArgs
            {
                NavigationType = intentType ?? NavigatingTypes.Push,
                NewContent = view,
                OldContent = oldFrame?.ViewWrapper.View,
                IsAnimating = isAnimated,
                HasBackButton = NavigationStack.Count > 1,
                SafeArea = _scaffold.SafeArea,
            });

            if (oldFrame is View oldFrameView)
                oldFrameView.IsVisible = false;

            oldFrame?.TryDisappearing(true);
            view.TryAppearing(true);

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
            });

            _frames.Remove(currentFrame);
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

            var frame = Frames[oldIndex];
            _frames.RemoveAt(oldIndex);
            _navigationStack.RemoveAt(oldIndex);
            await PushAsync(newView, isAnimated, frame, NavigatingTypes.Replace);
            _scaffold.Children.Remove((View)frame);
            return true;
        }

        internal bool RemoveView(int index)
        {
            if (index < 0 || index >= NavigationStack.Count)
                return false;

            var frame = Frames[index];
            _frames.RemoveAt(index);
            _navigationStack.RemoveAt(index);
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

                _frames.Insert(index, frame);
                _scaffold.Children.Insert(index, (View)frame);
                _navigationStack.Insert(index, view);

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
    }
}
