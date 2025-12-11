namespace GrabWorkerService
{
    /// <summary>
    /// 取像站設定（Top / Bottom + WorkerId）
    /// </summary>
    public class GrabWorkerOptions
    {
        /// <summary>
        /// 線別 Group，例如 1 = Line1
        /// </summary>
        public int GroupId { get; set; } = 1;

        /// <summary>
        /// 取像站位置：Top 或 Bottom
        /// </summary>
        public string Side { get; set; } = "Top";   // "Top" / "Bottom"

        /// <summary>
        /// 取像站編號，例如 1、2、3 ...
        /// routing key = aoi.grabworker.{group}.{side}.{id}
        /// </summary>
        public int WorkerId { get; set; } = 1;

        /// <summary>
        /// 模擬 Trigger 間隔（毫秒）
        /// </summary>
        public int MockTriggerIntervalMs { get; set; } = 120;
    }
}
