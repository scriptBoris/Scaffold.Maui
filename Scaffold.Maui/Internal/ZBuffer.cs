using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScaffoldLib.Maui.Core;
using Microsoft.Maui.Handlers;
using System.Threading;

namespace ScaffoldLib.Maui.Internal;

internal class ZBuffer : Layout, IZBuffer, ILayoutManager, IDisposable
{
    private readonly List<LayerItem> _items = new();
    private readonly IScaffold _scaffold;
    private readonly Dictionary<int, LayerItem> _backgrounds = new();

    public ZBuffer(IScaffold scaffoldContext)
    {
        _scaffold = scaffoldContext;
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

    public Task AddLayerAsync(IZBufferLayout newLayer, int zIndex, bool animation)
    {
        if (!MainThread.IsMainThread)
            throw new InvalidOperationException("Please ivoke in main UI thread");

        var old = _items.LastOrDefault((x) => x.Index == zIndex);
        var nev = new LayerItem(newLayer, zIndex, this);

        if (old == null)
            TryCreateBackgroundLayer(zIndex, animation);

        _items.Add(nev);
        Children.Add(newLayer);

        _ = old?.HideAsync(animation);
        return nev.ShowAsync(animation);
    }

    public void AddLayer(IZBufferLayout newLayer, int zIndex, bool animation)
    {
        if (!MainThread.IsMainThread)
            throw new InvalidOperationException("Please ivoke in main UI thread");

        var old = _items.LastOrDefault((x) => x.Index == zIndex);
        var nev = new LayerItem(newLayer, zIndex, this);

        _items.Add(nev);
        Children.Add(nev.View);

        if (animation)
        {
            _ = old?.HideAsync(true);
            _ = nev.ShowAsync(true);

            if (old == null)
                _ = TryCreateBackgroundLayerAsync(zIndex, true);
        }
        else
        {
            old?.Hide(false);
            nev.Show(false);

            if (old == null)
                TryCreateBackgroundLayer(zIndex, false);
        }
    }

    public Task RemoveLayer(IZBufferLayout removedOverlay, bool animation)
    {
        if (!MainThread.IsMainThread)
            throw new InvalidOperationException("Please ivoke in main UI thread");

        var layer = _items.LastOrDefault(x => x.ZView == removedOverlay);
        if (layer == null)
            return Task.CompletedTask;

        return RemoveLayerAsync(layer, animation);
    }

    public async Task RemoveLayers(int zIndex, bool animation)
    {
        var layers = _items.Where(x => x.Index == zIndex).ToArray();
        foreach (var item in layers)
        {
            _ = RemoveLayerAsync(item, animation);
        }
        await TryHideBackgroundLayerAsync(zIndex, animation);
    }

    public IZBufferLayout? GetActualModalLayer()
    {
        var match = _items.LastOrDefault(x => x.View is IModalLayout);
        if (match == null)
        {
            return null;
        }
        else
        {
            return match.ZView;
        }
    }

    public async Task<bool?> TryPopModal(bool animation)
    {
        var modal = GetActualModalLayer();
        if (modal == null)
            return null;

        if (modal is IBackButtonListener bblistener)
        {
            if (!await bblistener.OnBackButton())
                return false;
        }

        await RemoveLayer(modal, animation);
        return true;
    }

    public async Task<bool> TryPopLayerAsync(int zIndex, bool animation)
    {
        var match = _items.LastOrDefault(x => x.Index == zIndex);
        if (match == null)
            return false;

        await RemoveLayerAsync(match, animation);
        return true;
    }

    private Task RemoveLayerAsync(LayerItem sureExistLayer, bool animation)
    {
        if (!MainThread.IsMainThread)
            throw new InvalidOperationException("Please ivoke in main UI thread");

        // Если слой уже удаляется, то ничего не делаем
        if (sureExistLayer.IsHiding)
            return Task.CompletedTask;

        bool useAnimation = animation;
        var popLayer = sureExistLayer;
        var currentLayer = _items.LastOrDefault(x => x.Index == popLayer.Index);

        // Если слой невиден
        if (currentLayer != popLayer)
        {
            _items.Remove(popLayer);
            Children.Remove(popLayer.View);
            popLayer.ZView.OnRemoved();
            return Task.CompletedTask;
        }

        _items.Remove(popLayer);
        var under = _items.LastOrDefault(x => x.Index == popLayer.Index);

        if (animation)
        {
            var taskHide = popLayer.HideAsync(true)
                .ContinueWithInUIThread(() =>
                {
                    Children.Remove(popLayer.View);
                    popLayer.ZView.OnRemoved();
                });

            _ = under?.ShowAsync(animation);

            if (under == null)
                _ = TryHideBackgroundLayerAsync(popLayer.Index, animation);

            return taskHide;
        }
        else
        {
            popLayer.Hide(animation);
            under?.Show(animation);
            Children.Remove(popLayer.View);
            popLayer.ZView.OnRemoved();
            
            if (under == null)
                TryHideBackgroundLayer(popLayer.Index, animation);

            return Task.CompletedTask;
        }
    }

    private void RemoveLayer(LayerItem sureExistLayer, bool animation)
    {
        if (!MainThread.IsMainThread)
            throw new InvalidOperationException("Please ivoke in main UI thread");

        // Если слой уже удаляется, то ничего не делаем
        if (sureExistLayer.IsHiding)
            return;

        bool useAnimation = animation;
        var popLayer = sureExistLayer;
        var currentLayer = _items.LastOrDefault(x => x.Index == popLayer.Index);

        // Если слой невиден
        if (currentLayer != popLayer)
        {
            _items.Remove(popLayer);
            Children.Remove(popLayer.View);
            popLayer.ZView.OnRemoved();
            return;
        }

        bool isRemoved = _items.Remove(popLayer);
        if (!isRemoved)
        {
            //System.Diagnostics.Debugger.Break();
        }

        var under = _items.LastOrDefault(x => x.Index == popLayer.Index);

        if (animation)
        {
            _ = popLayer.HideAsync(true)
                .ContinueWithInUIThread(() =>
                {
                    Children.Remove(popLayer.View);
                    popLayer.ZView.OnRemoved();
                });
            _ = under?.ShowAsync(true);

            if (under == null)
                _ = TryHideBackgroundLayerAsync(popLayer.Index, animation);
        }
        else
        {
            popLayer.Hide(false);
            under?.Show(false);
            Children.Remove(popLayer.View);
            popLayer.ZView.OnRemoved();

            if (under == null)
                TryHideBackgroundLayer(popLayer.Index, animation);
        }
    }

    private void TryCreateBackgroundLayer(int zindex, bool animation)
    {
        LayerItem? layerBG = null;
        if (_backgrounds.TryGetValue(zindex, out var already))
        {
            layerBG = already;
        }
        else
        {
            var layer = _scaffold.ViewFactory.CreateSharedModalBackground(new Args.CreateSharedModalBackground
            {
                ZIndex = zindex
            });

            if (layer != null)
            {
                try
                {
                    layerBG = new LayerItem(layer, zindex, this);
                    _backgrounds.Add(zindex, layerBG);
                    Children.Insert(0, layerBG.View);
                    layer.ZBufferIndex = zindex;
                    layer.TappedToOutside += Layer_TappedToOutside;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Fail to use view", ex);
                }
            }
        }
        
        if (layerBG == null)
            return;

        _ = layerBG.ShowAsync(animation);
    }

    private Task TryCreateBackgroundLayerAsync(int zindex, bool animation)
    {
        LayerItem? layerBG = null;
        if (_backgrounds.TryGetValue(zindex, out var already))
        {
            layerBG = already;
        }
        else
        {
            var layer = _scaffold.ViewFactory.CreateSharedModalBackground(new Args.CreateSharedModalBackground
            {
                ZIndex = zindex
            });

            if (layer != null)
            {
                try
                {
                    layerBG = new LayerItem(layer, zindex, this);
                    _backgrounds.Add(zindex, layerBG);
                    Children.Insert(0, layerBG.View);
                    layer.ZBufferIndex = zindex;
                    layer.TappedToOutside += Layer_TappedToOutside;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Fail to use view", ex);
                }
            }
        }

        if (layerBG == null)
            return Task.CompletedTask;

        return layerBG.ShowAsync(animation);
    }

    private Task TryHideBackgroundLayerAsync(int zindex, bool animation)
    {
        if (_backgrounds.TryGetValue(zindex, out var bgLayer))
        {
            return bgLayer.HideAsync(animation);    
        }

        return Task.CompletedTask;
    }

    private void TryHideBackgroundLayer(int zindex, bool animation)
    {
        if (_backgrounds.TryGetValue(zindex, out var bgLayer))
        {
            bgLayer.Hide(animation);
        }
    }

    private void Layer_TappedToOutside(ISharedModalBackground invoker, EventArgs e)
    { 
        int index = invoker.ZBufferIndex;
        var visible = _items.LastOrDefault(x => x.Index == index);
        if (visible != null)
        {
            visible.ZView.OnTapToOutside();
        }
    }

    public Size ArrangeChildren(Rect bounds)
    {
        foreach (var item in Children)
        {
            item.Arrange(bounds);
        }
        return bounds.Size;
        //foreach (var item in _backgrounds)
        //{
        //    item.Value.View.Arrange(bounds);
        //}

        //foreach (var item in _items)
        //{
        //    double x;
        //    double y;
        //    double width;
        //    double height;
        //    var view = item.View;
        //    switch (item.MauiView.VerticalOptions.Alignment)
        //    {
        //        case LayoutAlignment.Start:
        //            y = 0;
        //            height = view.DesiredSize.Height;
        //            break;
        //        case LayoutAlignment.Center:
        //            y = (bounds.Height / 2) - (view.DesiredSize.Height / 2);
        //            height = view.DesiredSize.Height;
        //            break;
        //        case LayoutAlignment.End:
        //            y = bounds.Height - view.DesiredSize.Height;
        //            height = view.DesiredSize.Height;
        //            break;
        //        default:
        //            y = 0;
        //            height = bounds.Height;
        //            break;
        //    }

        //    switch (item.MauiView.HorizontalOptions.Alignment)
        //    {
        //        case LayoutAlignment.Start:
        //            x = 0;
        //            width = view.DesiredSize.Width;
        //            break;
        //        case LayoutAlignment.Center:
        //            x = (bounds.Width / 2) - (view.DesiredSize.Width / 2);
        //            width = view.DesiredSize.Width;
        //            break;
        //        case LayoutAlignment.End:
        //            x = bounds.Width - view.DesiredSize.Width;
        //            width = view.DesiredSize.Width;
        //            break;
        //        default:
        //            x = 0;
        //            width = bounds.Width;
        //            break;
        //    }

        //    var r = new Rect(x, y, width, height);
        //    view.Arrange(r);
        //}
        //return bounds.Size;
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        foreach (var item in Children)
        {
            item.Measure(widthConstraint, heightConstraint);
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
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly ZBuffer _parent;

        public LayerItem(IView view, int zindex, ZBuffer parent)
        {
            _parent = parent;

            Index = zindex;
            View = view;
            ZView = (IZBufferLayout)view;
            MauiView = (View)view;

            if (view is IZBufferLayout layout)
                layout.DeatachLayer += OnViewDeatached;

        }

        public View MauiView { get; private set; }
        public IView View { get; private set; }
        public IZBufferLayout ZView { get; private set; }
        public int Index { get; private set; }

        public bool IsHiding => State == AppearingStates.Disappearing;
        public bool IsHided => State == AppearingStates.Disappear;
        public bool IsShowing => State == AppearingStates.Appearing;
        public bool IsShowed => State == AppearingStates.Appear;

        public AppearingStates State { get; private set; }
        public CancellationToken FetchCancelation
        {
            get
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new();
                return _cancellationTokenSource.Token;
            }
        }

        private async void OnViewDeatached()
        {
            await _parent.RemoveLayerAsync(this, true);
        }

        public void Show(bool animation)
        {
            var cancel = FetchCancelation;
            State = AppearingStates.Appearing;
            MauiView.TryAppearing();
            MauiView.IsVisible = true;

            if (animation)
            {
                ZView.OnShow(cancel).ContinueWithInUIThread(() =>
                {
                    if (!cancel.IsCancellationRequested)
                    {
                        MauiView.TryAppearing(true);
                        State = AppearingStates.Appear;
                    }
                });
            }
            else
            {
                ZView.OnShow();
                MauiView.TryAppearing(true);
                State = AppearingStates.Appear;
            }
        }

        public async Task ShowAsync(bool animation)
        {
            if (!animation)
            {
                Show(false);
                return;
            }

            var cancel = FetchCancelation;
            State = AppearingStates.Appearing;
            MauiView.TryAppearing();
            MauiView.IsVisible = true;

            await ZView.OnShow(cancel);

            if (!cancel.IsCancellationRequested)
            {
                MauiView.TryAppearing(true);
                State = AppearingStates.Appear;
            }
        }

        public void Hide(bool animation)
        {
            var cancel = FetchCancelation;
            State = AppearingStates.Disappearing;
            MauiView.TryDisappearing();

            if (animation)
            {
                ZView.OnHide(cancel).ContinueWithInUIThread(() =>
                {
                    if (!cancel.IsCancellationRequested)
                    {
                        MauiView.TryDisappearing(true);
                        MauiView.IsVisible = false;
                        State = AppearingStates.Disappear;
                    }
                });
            }
            else
            {
                ZView.OnHide();
                MauiView.TryDisappearing(true);
                MauiView.IsVisible = false;
                State = AppearingStates.Disappear;
            }
        }

        public async Task<bool> HideAsync(bool animation)
        {
            if (!animation)
            {
                Hide(false);
                return true;
            }

            var cancel = FetchCancelation;
            State = AppearingStates.Disappearing;
            MauiView.TryDisappearing();
            await ZView.OnHide(cancel);

            if (cancel.IsCancellationRequested)
                return false;

            MauiView.TryDisappearing(true);
            MauiView.IsVisible = false;
            State = AppearingStates.Disappear;
            return true;
        }

        public void Dispose()
        {
            if (View is IZBufferLayout layout)
                layout.DeatachLayer -= OnViewDeatached;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
