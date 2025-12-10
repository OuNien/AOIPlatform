using AOI.Common.Messages;

namespace UI.GatewayApi
{
    public class ResultStore
    {
        private readonly Dictionary<string, PanelInspectionCompleted> _results = new();

        public void Save(PanelInspectionCompleted res)
        {
            _results[res.PanelId] = res;
        }

        public PanelInspectionCompleted? Get(string panelId)
        {
            _results.TryGetValue(panelId, out var res);
            return res;
        }
    }
}
