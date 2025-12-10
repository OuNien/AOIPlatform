using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Domain
{    public sealed class Panel
    {
        public string PanelId { get; init; } = default!;
        public string LotId { get; init; } = default!;
        public int FieldCount { get; init; }
        public string RecipeId { get; init; } = default!;
    }
}
