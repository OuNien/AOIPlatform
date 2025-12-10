using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public sealed class UiStartPanel
    {
        public string PanelId { get; init; } = default!;
        public string LotId { get; init; } = default!;
        public int FieldCount { get; init; }
        public string RecipeId { get; init; } = default!;
    }
}
