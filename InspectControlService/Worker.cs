using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace InspectControlService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;

        private readonly int _groupId;
        private readonly int _workerCount;
        private readonly string _subscribeKey;

        // PanelId -> expected task count
        private readonly Dictionary<string, int> _expectedCount = new();
        // PanelId -> completed task count
        private readonly Dictionary<string, int> _completedCount = new();
        // PanelId -> FieldId -> DefectCodes
        private readonly Dictionary<string, Dictionary<string, List<string>>> _defects = new();

        public Worker(
            ILogger<Worker> logger,
            IMessageBus messageBus,
            IOptions<InspectControlOptions> options)
        {
            _logger = logger;
            _messageBus = messageBus;

            _groupId = options.Value.GroupId;
            _workerCount = options.Value.WorkerCount;

            _subscribeKey = $"aoi.inspectcontrol.{_groupId}";

            _logger.LogInformation("[InspectCtrl-{Group}] 訂閱 {Key}",
                _groupId, _subscribeKey);

            _messageBus.SubscribeAsync<ImageMapped>(_subscribeKey, HandleImageMappedAsync);
            _messageBus.SubscribeAsync<InspectResult>(_subscribeKey, HandleInspectResultAsync);
        }

        /// <summary>
        /// 收到 Mapping → 發 InspectOrder 給 InspectWorker.*.*
        /// </summary>
        private async Task HandleImageMappedAsync(ImageMapped mapped)
        {
            _logger.LogInformation(
                "[InspectCtrl-{Group}] 收到 ImageMapped Panel={Panel}, Field={Field}, Image={Image}, Step={Step}",
                _groupId, mapped.PanelId, mapped.FieldId, mapped.ImageId, mapped.Step);

            var order = new InspectOrder
            {
                PanelId = mapped.PanelId,
                FieldId = mapped.FieldId,
                ImageId = mapped.ImageId,
                RecipeId = mapped.RecipeId,
                Step = mapped.Step
            };

            // 初始化統計資料
            if (!_expectedCount.ContainsKey(mapped.PanelId))
            {
                _expectedCount[mapped.PanelId] = 0;
                _completedCount[mapped.PanelId] = 0;
                _defects[mapped.PanelId] = new Dictionary<string, List<string>>();
            }

            _expectedCount[mapped.PanelId]++;

            // 決定要派給哪一台 InspectWorker
            int workerTarget = (_expectedCount[mapped.PanelId] % _workerCount);

            string routingKey = $"aoi.inspectworker.{_groupId}.{workerTarget}";

            _logger.LogInformation(
                "[InspectCtrl-{Group}] 發出 InspectOrder → {Worker}  Panel={Panel} Field={Field}",
                _groupId, routingKey, order.PanelId, order.FieldId);

            await _messageBus.PublishAsync(order, routingKey);
        }

        /// <summary>
        /// 收到 InspectResult → 累計 → 若此 Panel 全部完成 → 發 PanelInspectionCompleted 給 UI
        /// </summary>
        private async Task HandleInspectResultAsync(InspectResult result)
        {
            _logger.LogInformation(
                "[InspectCtrl-{Group}] 收到 InspectResult Panel={Panel}, Field={Field}, Defects={Count}",
                _groupId, result.PanelId, result.FieldId, result.DefectCodes.Count);

            if (!_defects.TryGetValue(result.PanelId, out var fieldMap))
            {
                fieldMap = new Dictionary<string, List<string>>();
                _defects[result.PanelId] = fieldMap;
            }

            if (!fieldMap.TryGetValue(result.FieldId, out var defectList))
            {
                defectList = new List<string>();
                fieldMap[result.FieldId] = defectList;
            }

            defectList.AddRange(result.DefectCodes);

            _completedCount[result.PanelId]++;

            // 檢查 Panel 是否全部完成
            if (_completedCount[result.PanelId] >= _expectedCount[result.PanelId])
            {
                _logger.LogInformation(
                    "[InspectCtrl-{Group}] Panel={Panel} 所有檢測完成 → 發 PanelInspectionCompleted",
                    _groupId, result.PanelId);

                var completed = new PanelInspectionCompleted
                {
                    PanelId = result.PanelId,
                    DefectsByField = fieldMap
                };

                // 發到 UI Gateway
                string routingKey = $"aoi.gateway.{_groupId}";
                await _messageBus.PublishAsync(completed, routingKey);

                // 清理記憶體
                _expectedCount.Remove(result.PanelId);
                _completedCount.Remove(result.PanelId);
                _defects.Remove(result.PanelId);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[InspectCtrl-{Group}] 檢測控制站啟動完成", _groupId);
            return Task.CompletedTask;
        }
    }
}
