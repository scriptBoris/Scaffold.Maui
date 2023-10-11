using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core
{
    public interface IDisplayActionSheetResult<T> : IDisplayActionSheetResult
    {
        new T? SelectedItem { get; set; }
    }

    public interface IDisplayActionSheetResult
    {
        bool IsCanceled { get; set; }
        bool IsDestruction { get; set; }
        object? SelectedItem { get; set; }
        int? SelectedItemId { get; set; }
        bool HasSelectedItem => SelectedItem != null || SelectedItemId != null;
    }

    internal class DisplayActionSheetResult<T> : IDisplayActionSheetResult<T>
    {
        public bool IsCanceled { get; set; }
        public bool IsDestruction { get; set; }
        public T? SelectedItem { get; set; }
        public int? SelectedItemId { get; set; }
        object? IDisplayActionSheetResult.SelectedItem 
        {
            get => SelectedItem; 
            set => SelectedItem = (T)value;
        }
    }

    internal class DisplayActionSheetResult : IDisplayActionSheetResult
    {
        public bool IsCanceled { get; set; }
        public bool IsDestruction { get; set; }
        public object? SelectedItem { get; set; }
        public int? SelectedItemId { get; set; }
    }
}
