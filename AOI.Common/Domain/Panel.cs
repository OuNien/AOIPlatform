using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Domain
{    public sealed class Panel
    {
        public PanelSide PanelSideTop { get; init; } = default!;
        public PanelSide PanelSideBottom { get; init; } = default!;
    }
}
