using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Toolkit;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScaffoldLib.Maui.Containers.Cupertino;

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

    public DisplayActionSheetLayer(string? title, string? cancel, string? destruction, string? displayProperty, object[] buttons)
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
            items.Add(i, buttons[i].GetDisplayItemText(displayProperty));
        BindableLayout.SetItemsSource(itemList, items);

        // label
        labelTitle.IsVisible = title != null;
        labelTitle.Text = title;

        // button cancel
        labelButtonCancel.Text = cancel;
        containerButtonCancel.IsVisible = cancel != null;
        buttonCancel.TapCommand = new Command(() =>
        {
            if (!IsBusy)
            {
                isCanceled = true;
                _ = Close();
            }
        });

        // button destruction
        labelButtonDestruction.Text = destruction;
        containerButtonDestruction.IsVisible = destruction != null;
        buttonDestruction.TapCommand = new Command(() =>
        {
            if (!IsBusy)
            {
                isDestruction = true;
                _ = Close();
            }
        });

        // Safe area
        Scaffold.SafeAreaChanged += UpdateSafeArea;
        UpdateSafeArea(null, Scaffold.SafeArea);

        // BG
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
    }

    public ICommand CommandTapItem { get; private set; }

    public event VoidDelegate? DeatachLayer;

    private void UpdateSafeArea(object? sender, Thickness e)
    {
        var padding = new Thickness(e.Left, e.Top, e.Right, e.Bottom * 0.7);
        Padding = padding;
    }

    public async Task Show()
    {
        rootStackLayout.TranslationY = rootStackLayout.Height;
        isAnimatingShow = true;
        await Task.WhenAll(
            this.FadeTo(1, 180),
            rootStackLayout.TranslateTo(0, 0, 180, Easing.SinInOut)
        );
        isAnimatingShow = false;
    }

    public async Task Close()
    {
        if (isAnimatingShow)
            this.CancelAnimations();

        if (isAnimatingClose)
            return;

        isAnimatingClose = true;

        await Task.WhenAll(
            this.FadeTo(0, 120),
            rootStackLayout.TranslateTo(0, Height, 120, Easing.SinInOut)
        );

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
}