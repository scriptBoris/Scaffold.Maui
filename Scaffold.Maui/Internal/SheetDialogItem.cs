using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScaffoldLib.Maui.Internal;

internal class SheetDialogItem : INotifyPropertyChanged
{
    private bool _isSelected;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public required int LogicElementIndex { get; set; }
    public required ICommand TapCommand { get; set; }
    public required string DisplayedText { get; set; }
    public required object LogicItem { get; set; }
}