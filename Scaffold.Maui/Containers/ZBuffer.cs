using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scaffold.Maui.Core;
using Scaffold.Maui.Internal;

namespace Scaffold.Maui.Containers;

public class ZBuffer : Layout, ILayoutManager, IDisposable
{
    private readonly List<LayerItem> items = new();

    public ZBuffer()
    {
        IsVisible = false;
    }

    public int LayersCount => items.Count;

    public async void AddLayer<T>(T layer, int zIndex) where T : View, IZBufferLayout
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

        await layer.Show();

        if (layer is IAppear appear)
            appear.OnAppear();
    }

    public async Task<bool> RemoveLayerAsync(int zIndex)
    {
        var m = items.LastOrDefault(x => x.Index == zIndex);
        if (m == null)
            return false;

        await RemoveLayerAsync(m);
        return true;
    }

    public Task Pop()
    {
        var last = items.LastOrDefault();
        if (last == null)
            return Task.CompletedTask;

        return RemoveLayerAsync(last);
    }

    private async Task RemoveLayerAsync(LayerItem layerItem)
    {
        items.Remove(layerItem);

        if (layerItem.View is IZBufferLayout layout)
            await layout.Close();

        if (layerItem.View is IAppear av)
            av.OnDisappear();

        layerItem.Dispose();
        Children.Remove(layerItem.View);

        if (items.Count == 0)
            IsVisible = false;
    }

    public Size ArrangeChildren(Rect bounds)
    {
        foreach (var item in items)
            ((IView)item.View).Arrange(bounds);

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
        public LayerItem(View view)
        {
            View = view;

            if (view is IZBufferLayout layout)
                layout.DeatachLayer += OnViewDeatached;
        }

        public View View { get; private set; }
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
