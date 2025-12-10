using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;

namespace InspectControlService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;
        private readonly string _stationName = "InspectCtrl-1";

        // PanelId -> expected task count
        private readonly Dictionary<string, int> _expectedCount = new();
        // PanelId -> completed task count
        private readonly Dictionary<string, int> _completedCount = new();
        // PanelId -> FieldId -> DefectCodes
        private readonly Dictionary<string, Dictionary<string, List<string>>> _defects = new();

        public Worker(ILogger<Worker> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;

            if (messageBus is RabbitMqMessageBus inMemory)
            {
                //_messageBus.Subscribe<ImageMapped>(HandleImageMappedAsync);
                //_messageBus.Subscribe<InspectResult>(HandleInspectResultAsync);
                _messageBus.Subscribe<ImageMapped>("aoi.inspectcontrol.1", HandleImageMappedAsync);
                _messageBus.Subscribe<InspectResult>("aoi.inspectcontrol.1", HandleInspectResultAsync);


            }
        }

        private async Task OnMessageAsync(object msg)
        {
            switch (msg)
            {
                case ImageMapped mapped:
                    await HandleImageMappedAsync(mapped);
                    break;
                case InspectResult result:
                    await HandleInspectResultAsync(result);
                    break;
            }
        }

        /// <summary>
        /// 收到 Mapping → 派檢測任務給 InspectWorker
        /// </summary>
        private async Task HandleImageMappedAsync(ImageMapped mapped)
        {
            _logger.LogInformation(
                "[{Station}] 收到 ImageMapped Panel={Panel}, Field={Field}, Image={Image}, Step={Step}",
                _stationName, mapped.PanelId, mapped.FieldId, mapped.ImageId, mapped.Step);

            // 建立 InspectOrder
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

            _logger.LogInformation(
                "[{Station}] 發出 InspectOrder Panel={Panel}, Field={Field}, Step={Step}",
                _stationName, order.PanelId, order.FieldId, order.Step);

            await _messageBus.PublishAsync(order);
        }

        /// <summary>
        /// 收到 InspectResult → 累計結果 → 判斷是否 Panel 完成
        /// </summary>
        private async Task HandleInspectResultAsync(InspectResult result)
        {
            _logger.LogInformation(
                "[{Station}] 收到檢測結果 Panel={Panel}, Field={Field}, Image={Image}, Step={Step}, Defects={Count}",
                _stationName, result.PanelId, result.FieldId, result.ImageId, result.Step, result.DefectCodes.Count);

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

            if (_completedCount.ContainsKey(result.PanelId))
            {
                _completedCount[result.PanelId]++;
            }
            else
            {
                _completedCount[result.PanelId] = 1;
            }

            // 檢查是否該 Panel 所有檢測都完成
            if (_expectedCount.TryGetValue(result.PanelId, out var expected) &&
                _completedCount[result.PanelId] >= expected)
            {
                _logger.LogInformation(
                    "[{Station}] Panel={Panel} 所有檢測完成，開始合併並發出 PanelInspectionCompleted",
                    _stationName, result.PanelId);

                var completed = new PanelInspectionCompleted
                {
                    PanelId = result.PanelId,
                    DefectsByField = fieldMap
                };

                await _messageBus.PublishAsync(completed);

                // 依需求可選擇保留或清掉記憶體
                _expectedCount.Remove(result.PanelId);
                _completedCount.Remove(result.PanelId);
                _defects.Remove(result.PanelId);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{Station}] InspectControlService 啟動，等待 mapping / 檢測結果…", _stationName);
            return Task.CompletedTask;
        }
    }
}
