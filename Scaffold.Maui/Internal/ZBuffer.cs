using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScaffoldLib.Maui.Core;
using Microsoft.Maui.Handlers;

namespace ScaffoldLib.Maui.Internal;

internal class ZBuffer : Layout, IZBuffer, ILayoutManager, IDisposable
{
    private readonly List<LayerItem> _items = new();

    public ZBuffer()
    {
        BindingContext = null;

        // on winui wtf empty zbuffer cannot pass interactive events
#if WINDOWS && NET8_0_OR_GREATER
        InputTransparent = true;
        CascadeInputTransparent = false;
#endif
    }

    public IReadOnlyList<View> Layers => _items.Select(x => (View)x.View).ToList();

    public int LayersCount => _items.Count;

#if IOS
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is LayoutHandler h)
        {
            h.PlatformView.BackgroundColor = null;
            h.PlatformView.UserInteractionEnabled = false;
        }
    }
#endif

    public async void AddLayer(IZBufferLayout newLayer, int zIndex)
    {
        BatchBegin();

        var old = _items.LastOrDefault((x) => x.Index == zIndex);
        var newLayerItem = new LayerItem(newLayer)
        {
            Buffer = this,
            Index = zIndex,
        };
        _items.Add(newLayerItem);
        Children.Add(newLayer);

        newLayer.TryAppearing();

        if (newLayer is View v)
            await v.AwaitReady();

        BatchCommit();

        var taskOld = Task.CompletedTask;
        if (old != null)
        {
            taskOld = this.Dispatcher.DispatchAsync(async () =>
            {
                if (old.View is not IZBufferLayout oldz ||
                    old.View is not View oldv)
                    return;

                oldz.TryDisappearing(false);
                await oldz.OnHide(old.FetchCancelation);
                oldv.IsVisible = false;
                oldz.TryDisappearing(true);
            });
        }

        var taskNew = this.Dispatcher.DispatchAsync(async () =>
        {
            await newLayer.OnShow(newLayerItem.FetchCancelation);
            newLayer.TryAppearing(true);
        });

        await Task.WhenAny(taskOld, taskNew);
    }

    public async Task<bool> RemoveLayerAsync(int zIndex)
    {
        var match = _items.LastOrDefault(x => x.Index == zIndex);
        if (match == null)
            return false;

        await RemoveLayerAsync(match);
        var under = _items.LastOrDefault(x => x.Index == zIndex);
        if (under != null)
            ShowLayer(under);

        return true;
    }

    public bool Pop()
    {
        var last = _items.LastOrDefault(x => x.View is IModalLayout);
        if (last == null)
            return false;

        RemoveLayerAsync(last).ConfigureAwait(false);
        var under = _items.LastOrDefault(x => x.View is IModalLayout);
        if (under != null)
            ShowLayer(under);

        return true;
    }

    private async void PopAsync(LayerItem popLayer)
    {
        popLayer.IsRemoving = true;
        _items.Remove(popLayer);

        if (popLayer.View is View v && v.IsVisible)
        {
            var under = _items.LastOrDefault(x => x.Index == popLayer.Index);

            popLayer.View.TryDisappearing();

            if (popLayer.View is IZBufferLayout layout)
                await layout.OnHide(popLayer.FetchCancelation);

            popLayer.View.TryDisappearing(true);

            if (under != null)
                ShowLayer(under);
        }

        if (popLayer.View is IZBufferLayout layout2)
            layout2.OnRemoved();

        Children.Remove(popLayer.View);
        popLayer.Dispose();
    }

    private async Task RemoveLayerAsync(LayerItem removedLayer)
    {
        removedLayer.IsRemoving = true;
        _items.Remove(removedLayer);

        if (removedLayer.View is View v && v.IsVisible)
        {
            removedLayer.View.TryDisappearing();

            if (removedLayer.View is IZBufferLayout layout)
                await layout.OnHide(removedLayer.FetchCancelation);

            removedLayer.View.TryDisappearing(true);
        }

        if (removedLayer.View is IZBufferLayout layout2)
            layout2.OnRemoved();

        Children.Remove(removedLayer.View);
        removedLayer.Dispose();
    }

    private async void ShowLayer(LayerItem show)
    {
        if (show.View is not IZBufferLayout showz ||
            show.View is not View showv)
            return;

        showv.IsVisible = true;
        showz.TryAppearing();
        await showz.OnShow(show.FetchCancelation);
        showz.TryAppearing(true);
    }

    public Size ArrangeChildren(Rect bounds)
    {
        //foreach (var item in _items)
        //    item.View.Arrange(bounds);
        foreach (var item in Children)
        {
            var s = item.Arrange(bounds);
        }

        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        foreach (View item in Children)
        {
            var s = item.Measure(widthConstraint, heightConstraint);
        }

        return new Size(widthConstraint, heightConstraint);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    public void Dispose()
    {
        foreach (var item in _items)
            item.Dispose();

        Children.Clear();
    }

    private class LayerItem : IDisposable
    {
        private CancellationTokenSource cancellationTokenSource = new();

        public LayerItem(IView view)
        {
            View = view;

            if (view is IZBufferLayout layout)
                layout.DeatachLayer += OnViewDeatached;
        }

        public IView View { get; private set; }
        public required ZBuffer Buffer { get; set; }
        public required int Index { get; set; }
        public bool IsRemoving { get; set; }
        public AppearingStates State { get; set; }
        public CancellationToken FetchCancelation
        {
            get
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new();
                return cancellationTokenSource.Token;
            }
        }

        private void OnViewDeatached()
        {
            if (IsRemoving)
                return;

            Buffer.PopAsync(this);
        }

        public void Dispose()
        {
            if (View is IZBufferLayout layout)
                layout.DeatachLayer -= OnViewDeatached;
        }
    }
}
