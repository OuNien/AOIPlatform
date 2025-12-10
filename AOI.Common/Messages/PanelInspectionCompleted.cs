using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public sealed class PanelInspectionCompleted
    {
        public string PanelId { get; init; } = default!;
        // FieldId -> Defect codes
        public Dictionary<string, List<string>> DefectsByField { get; init; } = new();
    }
}
