using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Core;
using ScaffoldLib.Maui.Internal;
using ScaffoldLib.Maui.Toolkit;
using System.Threading.Tasks;
using System.Windows.Input;
#if IOS
using UIKit;
#endif

namespace ScaffoldLib.Maui.Containers.Cupertino;

public partial class DisplayActionSheetLayer : IDisplayActionSheet
{
    private readonly TaskCompletionSource<IDisplayActionSheetResult> _tsc = new();
    private readonly object[] _originalItems;

    private bool isCanceled;
    private bool isDestruction;
    private int? prepareInt;
    private object? prepareSelectedItem;
    private bool isBusy;

    public event VoidDelegate? DeatachLayer;

    public DisplayActionSheetLayer(CreateDisplayActionSheet args)
    {
        string? title = args.Title;
        string? cancel = args.Cancel;
        string? destruction = args.Destruction;
        string? displayProperty = args.ItemDisplayBinding;
        object[] buttons = args.Items;

        _originalItems = buttons;
        CommandTapItem = new Command((param) =>
        {
            if (!isBusy && param is KeyValuePair<int, string> kvp)
            {
                prepareInt = kvp.Key;
                prepareSelectedItem = _originalItems[kvp.Key];
                DeatachLayer?.Invoke();
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
            if (!isBusy)
            {
                isCanceled = true;
                DeatachLayer?.Invoke();
            }
        });

        // button destruction
        labelButtonDestruction.Text = destruction;
        containerButtonDestruction.IsVisible = destruction != null;
        buttonDestruction.TapCommand = new Command(() =>
        {
            if (!isBusy)
            {
                isDestruction = true;
                DeatachLayer?.Invoke();
            }
        });

        // Safe area
        Scaffold.DeviceSafeAreaChanged += UpdateSafeArea;
        UpdateSafeArea(null, Scaffold.DeviceSafeArea);

        // BG
        this.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                if (!isBusy)
                {
                    isCanceled = true;
                    DeatachLayer?.Invoke();
                }
            }),
        });
    }

    public ICommand CommandTapItem { get; private set; }

    private void UpdateSafeArea(object? sender, Thickness e)
    {
        var padding = new Thickness(e.Left, e.Top, e.Right, e.Bottom * 0.7);
        Padding = padding;
    }

    public async Task OnShow(CancellationToken cancel)
    {
#if IOS
        isBusy = true;
        var tsc = new TaskCompletionSource();
        var root = (UIView)this.Handler!.PlatformView!;
        var stackView = (UIView)rootStackLayout.Handler!.PlatformView!;
        var originFrame = stackView.Frame;
        stackView.Frame = originFrame.OffsetBy(0, originFrame.Height);

        double dur = (double)250 / 1000.0;
        var animator = new UIViewPropertyAnimator(dur, UIViewAnimationCurve.EaseInOut,
        () =>
        {
            root.Alpha = 1;
            stackView.Frame = originFrame;
        });
        animator.UserInteractionEnabled = false;
        animator.AddCompletion(pos =>
        {
            rootStackLayout.TranslationY = 0;
            this.Opacity = 1;
            tsc.SetResult();
        });
        animator.StartAnimation();
        await tsc.Task;
        isBusy = false;
#endif
    }

    public async Task OnHide(CancellationToken cancel)
    {
        isBusy = true;
        this.CancelAnimations();

        await Task.WhenAll(
            this.FadeTo(0, 190),
            rootStackLayout.TranslateTo(0, Height, 190, Easing.SinInOut)
        );
        isBusy = false;
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

    public Task<IDisplayActionSheetResult> GetResult()
    {
        return _tsc.Task;
    }
}