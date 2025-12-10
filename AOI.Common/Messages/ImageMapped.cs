using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public sealed class ImageMapped
    {
        public string PanelId { get; init; } = default!;
        public string FieldId { get; init; } = default!;
        public string ImageId { get; init; } = default!;
        public string RecipeId { get; init; } = default!;
        public int Step { get; init; }          // 行程編號
    }
}
