using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public sealed class CaptureOrder
    {
        public string PanelId { get; init; } = default!;
        public string FieldId { get; init; } = default!;
    }
}
