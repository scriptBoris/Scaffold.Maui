using Microsoft.Maui.Controls;
using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Toolkit;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScaffoldLib.Maui.Containers.Material;

public partial class DisplayActionSheetLayer : IDisplayActionSheet
{
    private readonly TaskCompletionSource<IDisplayActionSheetResult> _tsc = new();
    private readonly object[] _originalItems;

    private bool isBusy;
    private bool isCanceled;
    private bool isDestruction;
    private int? prepareInt;
    private object? prepareSelectedItem;
    private SheetDialogItem? selectedItemByInit;

    public DisplayActionSheetLayer(CreateDisplayActionSheet args)
    {
        _originalItems = args.Items;
        CommandTapItem = new Command<SheetDialogItem>(OnItemSelected);
        InitializeComponent();
        OnHide();

        itemList.BindingContext = this;
        var items = new List<SheetDialogItem>();
        for (int i = 0; i < args.Items.Length; i++)
        {
            string text = args.Items[i].GetDisplayItemText(args.ItemDisplayBinding);
            var item = new SheetDialogItem
            {
                LogicElementIndex = i,
                LogicItem = args.Items[i],
                DisplayedText = text,
                TapCommand = CommandTapItem,
                IsSelected = args.SelectedItemId == i,
            };

            if (item.IsSelected)
                selectedItemByInit = item;
            
            items.Add(item);
        }
        BindableLayout.SetItemsSource(itemList, items);

        labelTitle.IsVisible = args.Title != null;
        labelTitle.Text = args.Title;
        labelDescription.IsVisible = args.Description != null;
        labelDescription.Text = args.Description;
        bool isTopVisible = args.Title != null || args.Description != null;
        topContainer.IsVisible = isTopVisible;
        labelTitleUnderline.IsVisible = isTopVisible;
        if (labelTitleUnderline.IsVisible)
        {
            labelTitleUnderline.HeightRequest = 1 / DeviceDisplay.Current.MainDisplayInfo.Density;
        }

        labelButtonCancel.Text = args.Cancel;
        buttonCancel.IsVisible = args.Cancel != null;
        buttonCancel.TapCommand = new Command(() =>
        {
            if (!isBusy)
            {
                isCanceled = true;
                DeatachLayer?.Invoke();
            }
        });

        labelButtonDestruction.Text = args.Destruction;
        buttonDestruction.IsVisible = args.Destruction != null;
        buttonDestruction.TapCommand = new Command(() =>
        {
            if (!isBusy)
            {
                isDestruction = true;
                DeatachLayer?.Invoke();
            }
        });

        if (args.Destruction == null && args.Cancel == null)
            buttonsContainer.IsVisible = false;
    }

    public ICommand CommandTapItem { get; }

    public event VoidDelegate? DeatachLayer;

    private async void OnItemSelected(SheetDialogItem item)
    {
        if (isBusy)
            return;

        isBusy = true;

        if (selectedItemByInit != null)
            selectedItemByInit.IsSelected = false;

        prepareInt = item.LogicElementIndex;
        prepareSelectedItem = item.LogicItem;
        item.IsSelected = true;

        await Task.Delay(250);
        DeatachLayer?.Invoke();
    }

    public async Task OnShow(CancellationToken cancel)
    {
        isBusy = true;
        await this.AnimateTo(
            start: Opacity,
            end: 1,
            name: nameof(OnShow),
            updateAction: (v, value) =>
            {
                v.Opacity = value;
                v.Scale = double.Lerp(0.95, 1.0, value);
            },
            length: 180,
            easing: null,
            cancel: cancel);
        isBusy = false;
    }

    public async Task OnHide(CancellationToken cancel)
    {
        isBusy = true;
        await this.AnimateTo(
            start: Opacity,
            end: 0,
            name: nameof(OnHide),
            updateAction: (v, value) =>
            {
                v.Opacity = value;
            },
            length: 180,
            easing: null,
            cancel: cancel);
        isBusy = false;
    }

    public void OnShow()
    {
        this.Opacity = 1;
        this.Scale = 1;
    }

    public void OnHide()
    {
        this.Opacity = 0;
        this.Scale = 0.95;
    }

    public Task<IDisplayActionSheetResult> GetResult()
    {
        return _tsc.Task;
    }

    public void OnRemoved()
    {
        _tsc.TrySetResult(new DisplayActionSheetResult
        {
            IsCanceled = isCanceled,
            IsDestruction = isDestruction,
            SelectedItemId = prepareInt,
            SelectedItem = prepareSelectedItem,
        });
    }

    public void OnTapToOutside()
    {
        isCanceled = true;
        DeatachLayer?.Invoke();
    }
}