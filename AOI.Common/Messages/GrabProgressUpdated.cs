namespace AOI.Common.Messages
{
    /// <summary>
    /// GrabControl → UI，用於通知 UI 目前抓取進度的事件。
    /// </summary>
    public sealed class GrabProgressUpdated
    {
        public string BatchId { get; init; } = default!; // 一次 GrabStart 就是一批
        public int GroupId { get; init; }

        // --- Top ---
        public int TopExpectedPanels { get; init; }
        public int TopCompletedPanels { get; init; }
        public int TopCurrentPanelId { get; init; }
        public Dictionary<int, DateTimeOffset?> TopPanelFirstSeen { get; init; } = new();
        public Dictionary<int, DateTimeOffset?> TopPanelLastSeen { get; init; } = new();

        // 每台站的狀態（Top）
        public Dictionary<string, int> TopStationLastPanel { get; init; } = new();
        public Dictionary<string, DateTimeOffset> TopStationLastSeen { get; init; } = new();

        // --- Bottom ---
        public int BottomExpectedPanels { get; init; }
        public int BottomCompletedPanels { get; init; }
        public int BottomCurrentPanelId { get; init; }
        public Dictionary<int, DateTimeOffset?> BottomPanelFirstSeen { get; init; } = new();
        public Dictionary<int, DateTimeOffset?> BottomPanelLastSeen { get; init; } = new();

        // 每台站的狀態（Bottom）
        public Dictionary<string, int> BottomStationLastPanel { get; init; } = new();
        public Dictionary<string, DateTimeOffset> BottomStationLastSeen { get; init; } = new();

        public DateTimeOffset Timestamp { get; init; }
    }
}
