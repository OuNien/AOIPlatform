using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Domain
{
    public sealed class PanelSide
    {   
        public string PartName { get; init; } = default!;
        public string LayerName { get; init; } = default!;
        public string StageName { get; init; } = default!;
        public string LotName { get; init; } = default!;
        public int PanelId { get; init; } = default!;
        public double PanelWidth { get; init; } = default!;
        public double PanelHeight { get; init; } = default!;
        public double PanelThick { get; init; } = default!;
        public string PanelMaterial { get; init; } = default!;
    }


}
