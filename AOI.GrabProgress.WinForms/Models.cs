using System;
using System.Collections.Generic;

namespace AOI.GrabProgressWinForms
{
    public class BatchInfo
    {
        public string BatchId { get; set; } = string.Empty;
    }

    public class GrabWorkerStatus
    {
        public string Name { get; set; } = string.Empty;
        public string Side { get; set; } = string.Empty; // Top / Bottom
        public string Status { get; set; } = string.Empty; // Idle / Grabbing / Done ...
        public int Frames { get; set; }
    }

}
