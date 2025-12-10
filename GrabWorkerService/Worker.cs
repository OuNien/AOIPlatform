using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace GrabWorkerService
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
            IOptions<GrabWorkerOptions> options)
        {
            _logger = logger;
            _messageBus = messageBus;

            _groupId = options.Value.GroupId;
            _workerId = options.Value.WorkerId;

            // 取像站的專屬 Queue = aoi.grabworker.{group}.{worker}
            _subscribeKey = $"aoi.grabworker.{_groupId}.{_workerId}";

            _logger.LogInformation("[GrabWorker G{G}-W{W}] 訂閱 {Key}",
                _groupId, _workerId, _subscribeKey);

            _messageBus.SubscribeAsync<CaptureOrder>(_subscribeKey, OnMessageAsync);
        }

        private async Task OnMessageAsync(CaptureOrder order)
        {
            _logger.LogInformation(
                "[GrabWorker G{G}-W{W}] 收到取像命令 Panel={Panel}, Field={Field}",
                _groupId, _workerId, order.PanelId, order.FieldId);

            // 模擬取像時間
            await Task.Delay(_random.Next(200, 500));

            var imageId = $"{order.PanelId}_{order.FieldId}_{Guid.NewGuid():N}";

            var captured = new ImageCaptured
            {
                PanelId = order.PanelId,
                FieldId = order.FieldId,
                ImageId = imageId,
                ImagePath = $@"\\dummy\images\{imageId}.png",
                CapturedAt = DateTimeOffset.Now
            };

            _logger.LogInformation(
                "[GrabWorker G{G}-W{W}] 完成取像 ImageId={ImageId}",
                _groupId, _workerId, captured.ImageId);

            // Publish 給 Mapping，同組：aoi.mapping.{group}
            string routingKey = $"aoi.mapping.{_groupId}";

            await _messageBus.PublishAsync(captured, routingKey);

            _logger.LogInformation(
                "[GrabWorker G{G}-W{W}] 已送出影像 → {RoutingKey}",
                _groupId, _workerId, routingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[GrabWorker G{G}-W{W}] 已啟動，等待取像命令…",
                _groupId, _workerId);
            return Task.CompletedTask;
        }
    }
}
