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
    private bool isAnimatingClose;
    private bool isAnimatingShow;

    private bool isCanceled;
    private bool isDestruction;
    private int? prepareInt;
    private object? prepareSelectedItem;

    private bool IsBusy => isAnimatingClose || isAnimatingShow;

    public DisplayActionSheetLayer(string? title, string? cancel, string? destruction, string? itemDisplayBinding, object[] buttons)
    {
        _originalItems = buttons;
        CommandTapItem = new Command((param) =>
        {
            if (!IsBusy && param is KeyValuePair<int, string> kvp)
            {
                prepareInt = kvp.Key;
                prepareSelectedItem = _originalItems[kvp.Key];
                _ = Close();
            }
        });
        InitializeComponent();
        Opacity = 0;
        itemList.BindingContext = this;
        var items = new Dictionary<int, string>();
        for (int i = 0; i < buttons.Length; i++)
            items.Add(i, buttons[i].GetDisplayItemText(itemDisplayBinding));
        BindableLayout.SetItemsSource(itemList, items);

        labelTitle.IsVisible = title != null;
        labelTitle.Text = title;

        labelButtonCancel.Text = cancel;
        buttonCancel.IsVisible = cancel != null;
        buttonCancel.TapCommand = new Command(() =>
        {
            if (!IsBusy)
            {
                isCanceled = true;
                _ = Close();
            }
        });

        labelButtonDestruction.Text = destruction;
        buttonDestruction.IsVisible = destruction != null;
        buttonDestruction.TapCommand = new Command(() =>
        {
            if (!IsBusy)
            {
                isDestruction = true;
                _ = Close();
            }
        });

        this.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                if (!IsBusy)
                {
                    isCanceled = true;
                    _ = Close();
                }
            }),
        });

        if (destruction == null && cancel == null)
            rootStackLayout.Padding = new Thickness(0, 10, 0, 0);
    }

    public ICommand CommandTapItem { get; private set; }

    public event VoidDelegate? DeatachLayer;

    public async Task Close()
    {
        if (isAnimatingShow)
            this.CancelAnimations();

        if (isAnimatingClose)
            return;

        isAnimatingClose = true;

        await this.FadeTo(0, 180);
        _tsc.TrySetResult(new DisplayActionSheetResult
        {
            IsCanceled = isCanceled,
            IsDestruction = isDestruction,
            SelectedItemId = prepareInt,
            SelectedItem = prepareSelectedItem,
        });
        DeatachLayer?.Invoke();
    }

    public Task<IDisplayActionSheetResult> GetResult()
    {
        return _tsc.Task;
    }

    public async Task Show()
    {
        isAnimatingShow = true;
        await this.FadeTo(1, 180);
        isAnimatingShow = false;
    }
}