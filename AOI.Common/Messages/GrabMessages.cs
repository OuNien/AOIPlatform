using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Common.Messages
{
    public class GrabTaskRequest
    {
        public string PanelId { get; set; }
        public int FieldIndex { get; set; }
        public string Recipe { get; set; }
    }

    public class GrabResult
    {
        public string PanelId { get; set; }
        public int FieldIndex { get; set; }
        public string ImagePath { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
