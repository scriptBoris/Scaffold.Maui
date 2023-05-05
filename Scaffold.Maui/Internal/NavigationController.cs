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
        private readonly List<Frame> _frames = new();
        private readonly ObservableCollection<View> _navigationStack = new();

        public NavigationController(ScaffoldView layout)
        {
            _scaffold = layout;
            NavigationStack = new(_navigationStack);
            Frames = new ReadOnlyCollection<Frame>(_frames);
        }

        public ReadOnlyCollection<Frame> Frames { get; private set; }
        public ReadOnlyObservableCollection<View> NavigationStack { get; private set; }
        public Frame? CurrentFrame => Frames.LastOrDefault();

        internal async Task<Frame> PushAsync(View view, bool isAnimated, Frame? currentFrame = null, NavigatingTypes? intentType = null)
        {
            var oldFrame = currentFrame ?? CurrentFrame;
            if (oldFrame == null)
                isAnimated = false;

            var frame = new Frame(view, _scaffold.ViewFactory);
            _frames.Add(frame);
            _scaffold.Children.Insert(_scaffold.Children.Count - 1, frame);
            _navigationStack.Add(view);

            await frame.UpdateVisual(new NavigatingArgs
            {
                NavigationType = intentType ?? NavigatingTypes.Push,
                NewContent = view,
                OldContent = oldFrame?.View,
                IsAnimating = isAnimated,
                HasBackButton = NavigationStack.Count > 1,
            });

            if (oldFrame != null)
            {
                oldFrame.IsVisible = false;
            }

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

            prevFrame.IsVisible = true;
            _navigationStack.Remove(currentFrame.View);

            await currentFrame.UpdateVisual(new NavigatingArgs
            {
                NavigationType = NavigatingTypes.Pop,
                NewContent = prevFrame.View,
                OldContent = currentFrame.View,
                IsAnimating = isAnimated,
                HasBackButton = hasBackButton,
            });

            _frames.Remove(currentFrame);
            _scaffold.Children.Remove(currentFrame);

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
            _scaffold.Children.Remove(frame);
            return true;
        }

        internal bool RemoveView(int index)
        {
            if (index < 0 || index >= NavigationStack.Count)
                return false;

            var frame = Frames[index];
            _frames.RemoveAt(index);
            _navigationStack.RemoveAt(index);
            _scaffold.Children.Remove(frame);
            return true;
        }
    }
}
