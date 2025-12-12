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

    public class GrabStatusResponse
    {
        public string BatchId { get; set; } = "";

        public int TopExpectedPanels { get; set; }
        public int TopCompletedPanels { get; set; }

        public int BottomExpectedPanels { get; set; }
        public int BottomCompletedPanels { get; set; }

        public Dictionary<string, int>? TopStationLastPanel { get; set; }
        public Dictionary<string, string>? TopStationLastSeen { get; set; }

        public Dictionary<string, int>? BottomStationLastPanel { get; set; }
        public Dictionary<string, string>? BottomStationLastSeen { get; set; }

        // 前端自行產生的 Workers 清單
        public List<GrabWorkerStatus> Workers { get; set; } = new();
    }

}
