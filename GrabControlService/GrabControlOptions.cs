namespace GrabControlService
{
    public class GrabControlOptions
    {
        public int GroupId { get; set; } = 1;

        public int TopWorkerCount { get; set; } = 1;

        public int BottomWorkerCount { get; set; } = 1;

        /// <summary>
        /// 正反面允許的時間差（毫秒），超過視為不同步
        /// </summary>
        public int SideSyncToleranceMs { get; set; } = 50;

        /// <summary>
        /// 同一 frame 等待所有站回報的 timeout（毫秒）
        /// 例如 2000 ms 內還沒收到所有站就視為異常
        /// </summary>
        public int FrameTimeoutMs { get; set; } = 2000;

        /// <summary>
        /// 單一工作站「很久沒回報」的 timeout（毫秒）
        /// 用來抓整台 camera 掛掉或卡住
        /// </summary>
        public int StationTimeoutMs { get; set; } = 5000;

        /// <summary>
        /// 某站允許「落後幾個 frame」的容忍值（通常 0 或 1）
        /// 超過就視為不合群
        /// </summary>
        public int StationLagToleranceFrames { get; set; } = 1;
    }
}
