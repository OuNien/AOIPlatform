using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public class ImageCaptured
    {
        public int PanelId { get; set; } = default!;
        public string Side { get; set; } = default!;  // "Top" / "Bottom"
       
        public string StationId { get; set; } = default!;
        public DateTimeOffset CapturedAt { get; set; }
    }

}
