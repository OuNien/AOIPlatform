using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public sealed class CaptureOrder
    {
        /// <summary>
        /// 要拍照的 Panel
        /// </summary>
        public int PanelId { get; init; } = default!;
    }
}
