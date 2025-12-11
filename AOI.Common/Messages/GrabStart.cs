using AOI.Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public sealed class GrabStart
    {
        public Panel GrabPanel { get; init; } = default!;
        public int ExpectedPanelCountTop { get; init; } = default!;
        public int ExpectedPanelCountBottom { get; init; } = default!;
    }
}
