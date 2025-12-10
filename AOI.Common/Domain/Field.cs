using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Domain
{
    public sealed class Field
    {
        public string PanelId { get; init; } = default!;
        public string FieldId { get; init; } = default!;
        public int Row { get; init; }
        public int Col { get; init; }
    }
}
