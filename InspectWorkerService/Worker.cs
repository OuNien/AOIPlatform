using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;

namespace InspectWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;
        private readonly Random _random = new();
        private readonly string _stationName;

        public Worker(ILogger<Worker> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;

            // 未來可以從設定檔讀，如 "Inspect-1"
            _stationName = "Inspect-1";

            if (messageBus is RabbitMqMessageBus mb)
            {
                //mb.Subscribe<InspectOrder>(HandleInspectOrderAsync);
                _messageBus.Subscribe<InspectOrder>($"aoi.inspectworker.{_stationName}", HandleInspectOrderAsync);

            }
        }

        private async Task OnMessageAsync(object msg)
        {
            if (msg is not InspectOrder order)
                return;

            await HandleInspectOrderAsync(order);
        }

        private async Task HandleInspectOrderAsync(InspectOrder order)
        {
            _logger.LogInformation(
                "[{Station}] 收到 InspectOrder Panel={Panel}, Field={Field}, Image={Image}, Step={Step}",
                _stationName, order.PanelId, order.FieldId, order.ImageId, order.Step);

            // 模擬 heavy compute（AI / Pattern Match 等）
            await Task.Delay(_random.Next(200, 600));

            var defectCount = _random.Next(0, 3);
            var defects = new List<string>();
            for (int i = 0; i < defectCount; i++)
            {
                defects.Add($"D{_random.Next(1, 1000):000}");
            }

            var result = new InspectResult
            {
                PanelId = order.PanelId,
                FieldId = order.FieldId,
                ImageId = order.ImageId,
                Step = order.Step,
                DefectCodes = defects
            };

            _logger.LogInformation(
                "[{Station}] 完成檢測 Panel={Panel}, Field={Field}, Image={Image}, Defects={Count}",
                _stationName, result.PanelId, result.FieldId, result.ImageId, result.DefectCodes.Count);

            await _messageBus.PublishAsync(result);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{Station}] InspectWorker 啟動，等待檢測任務…", _stationName);
            return Task.CompletedTask;
        }
    }
}
