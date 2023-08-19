using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Core
{
    public interface IDisplayActionSheetResult
    {
        public bool IsCanceled { get; set; }
        public bool IsDestruction { get; set; }
        public string? ItemText { get; set; }
        public int? ItemId { get; set; }
        public bool HasSelectedItem => ItemText != null || ItemId != null;
    }

    internal class DisplayActionSheetResult : IDisplayActionSheetResult
    {
        public bool IsCanceled { get; set; }
        public bool IsDestruction { get; set; }
        public string? ItemText { get; set; }
        public int? ItemId { get; set; }
    }
}
