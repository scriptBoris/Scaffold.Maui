using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScaffoldLib.Maui.Core;

namespace ScaffoldLib.Maui.Internal;

internal class ZBuffer : Layout, ILayoutManager, IDisposable
{
    private readonly List<LayerItem> items = new();

    public ZBuffer()
    {
        IsVisible = false;
    }

    public int LayersCount => items.Count;

    public async void AddLayer(IZBufferLayout layer, int zIndex)
    {
        IsVisible = true;

        var old = items.FirstOrDefault((x) => x.Index == zIndex);
        if (old != null)
        {
            items.Remove(old);
            Children.Remove(old.View);
            old.Dispose();
        }

        items.Add(new LayerItem(layer)
        {
            Buffer = this,
            Index = zIndex,
        });
        Children.Add(layer);

        layer.TryAppearing();
        await layer.Show();
        layer.TryAppearing(true);
    }

    public async Task<bool> RemoveLayerAsync(int zIndex)
    {
        var m = items.LastOrDefault(x => x.Index == zIndex);
        if (m == null)
            return false;

        await RemoveLayerAsync(m);
        return true;
    }

    public bool Pop()
    {
        var last = items.LastOrDefault();
        if (last == null)
            return false;

        RemoveLayerAsync(last).ConfigureAwait(false);
        return true;
    }

    private async Task RemoveLayerAsync(LayerItem layerItem)
    {
        items.Remove(layerItem);

        layerItem.View.TryDisappearing();

        if (layerItem.View is IZBufferLayout layout)
            await layout.Close();

        layerItem.View.TryDisappearing(true);

        Children.Remove(layerItem.View);
        layerItem.Dispose();

        if (items.Count == 0)
            IsVisible = false;
    }

    public Size ArrangeChildren(Rect bounds)
    {
        foreach (var item in items)
            item.View.Arrange(bounds);

        return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        foreach (var item in Children)
            item.Measure(widthConstraint, heightConstraint);

        return new Size(widthConstraint, heightConstraint);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    public void Dispose()
    {
        foreach (var item in items)
            item.Dispose();

        Children.Clear();
    }

    private class LayerItem : IDisposable
    {
        public LayerItem(IView view)
        {
            View = view;

            if (view is IZBufferLayout layout)
                layout.DeatachLayer += OnViewDeatached;
        }

        public IView View { get; private set; }
        public required ZBuffer Buffer { get; set; }
        public required int Index { get; set; }

        private void OnViewDeatached()
        {
            Buffer.RemoveLayerAsync(Index).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (View is IZBufferLayout layout)
                layout.DeatachLayer -= OnViewDeatached;
        }
    }
}
