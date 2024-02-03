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

    public DisplayActionSheetLayer(CreateDisplayActionSheet args)
    {
        _originalItems = args.Items;
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
        for (int i = 0; i < args.Items.Length; i++)
            items.Add(i, args.Items[i].GetDisplayItemText(args.ItemDisplayBinding));
        BindableLayout.SetItemsSource(itemList, items);

        labelTitle.IsVisible = args.Title != null;
        labelTitle.Text = args.Title;

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

        if (args.Destruction == null && args.Cancel == null)
            rootStackLayout.Padding = new Thickness(0, 10, 0, 0);
    }

    public ICommand CommandTapItem { get; private set; }

    public event VoidDelegate? DeatachLayer;

    public async Task OnShow(CancellationToken cancel)
    {
        this.CancelAnimations();
        isBusy = true;
        await this.FadeTo(1, 180);
        isBusy = false;
    }

    public async Task OnHide(CancellationToken cancel)
    {
        this.CancelAnimations();
        isBusy = true;
        await this.FadeTo(0, 180);
        isBusy = false;
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
}