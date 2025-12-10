using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public sealed class ImageCaptured
    {
        public string PanelId { get; init; } = default!;
        public string FieldId { get; init; } = default!;
        public string ImageId { get; init; } = default!;
        public string ImagePath { get; init; } = default!;
        public DateTimeOffset CapturedAt { get; init; }
    }
}
