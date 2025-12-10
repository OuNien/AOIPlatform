using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace InspectWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;
        private readonly Random _random = new();

        private readonly int _groupId;
        private readonly int _workerId;
        private readonly string _subscribeKey;

        public Worker(
            ILogger<Worker> logger,
            IMessageBus messageBus,
            IOptions<InspectWorkerOptions> options)
        {
            _logger = logger;
            _messageBus = messageBus;

            _groupId = options.Value.GroupId;
            _workerId = options.Value.WorkerId;

            // 訂閱 GPU Worker 專屬的 Queue
            _subscribeKey = $"aoi.inspectworker.{_groupId}.{_workerId}";

            _logger.LogInformation(
                "[InspectWorker G{G}-W{W}] 訂閱 {Key}",
                _groupId, _workerId, _subscribeKey);

            _messageBus.SubscribeAsync<InspectOrder>(_subscribeKey, HandleInspectOrderAsync);
        }

        private async Task HandleInspectOrderAsync(InspectOrder order)
        {
            _logger.LogInformation(
                "[InspectWorker G{G}-W{W}] 收到 InspectOrder Panel={Panel}, Field={Field}, Image={Image}, Step={Step}",
                _groupId, _workerId, order.PanelId, order.FieldId, order.ImageId, order.Step);

            // 模擬 GPU heavy compute
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
                "[InspectWorker G{G}-W{W}] 完成檢測 Panel={Panel}, Field={Field}, Image={Image}, Defects={Count}",
                _groupId, _workerId, result.PanelId, result.FieldId, result.ImageId, result.DefectCodes.Count);

            // 回傳給同組的 InspectControl
            string routingKey = $"aoi.inspectcontrol.{_groupId}";
            await _messageBus.PublishAsync(result, routingKey);

            _logger.LogInformation(
                "[InspectWorker G{G}-W{W}] 已送出結果 → {RoutingKey}",
                _groupId, _workerId, routingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "[InspectWorker G{G}-W{W}] GPU 檢測站啟動完成，等待檢測任務...",
                _groupId, _workerId);

            return Task.CompletedTask;
        }
    }
}
