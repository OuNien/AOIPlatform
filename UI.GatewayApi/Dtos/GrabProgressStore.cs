using AOI.Common.Messages;

namespace UI.GatewayApi.Dtos
{
    /// <summary>
    /// 存放 UI 需要的取像進度（採 In-Memory Cache）
    /// </summary>
    public class GrabProgressStore
    {
        private readonly Dictionary<string, GrabProgressUpdated> _batches = new();
        private readonly object _lock = new();

        public void Upsert(GrabProgressUpdated msg)
        {
            lock (_lock)
            {
                _batches[msg.BatchId] = msg;
            }
        }

        public GrabProgressUpdated? Get(string batchId)
        {
            lock (_lock)
            {
                return _batches.TryGetValue(batchId, out var progress)
                    ? progress
                    : null;
            }
        }

        public IEnumerable<GrabProgressUpdated> GetAll()
        {
            lock (_lock)
            {
                return _batches.Values.ToList();
            }
        }
    }
}
