using AOI.Common.Domain;
using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace SchedulerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;
        private readonly int _groupId;

        private readonly string _subscribeKey;

        public Worker(
            ILogger<Worker> logger,
            IMessageBus messageBus,
            IOptions<SchedulerOptions> options)
        {
            _logger = logger;
            _messageBus = messageBus;

            _groupId = options.Value.GroupId;

            // UI → Scheduler routing key
            _subscribeKey = $"aoi.scheduler.{_groupId}";

            _logger.LogInformation("[Scheduler-{Group}] 訂閱 {Key}",
                _groupId, _subscribeKey);

            _messageBus.SubscribeAsync<GrabStart>(_subscribeKey, HandleUiStartPanelAsync);
        }

        private async Task HandleUiStartPanelAsync(GrabStart grabStart)
        {
            _logger.LogInformation(
                "[Scheduler-{Group}] 收到 UI 開始",
                _groupId);

            _logger.LogInformation("[Scheduler-{Group}] 發送 Panel 排程 → {RoutingKey}",
                _groupId, $"aoi.grabcontrol.{_groupId}");

            await _messageBus.PublishAsync(grabStart, $"aoi.grabcontrol.{_groupId}.command");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "[Scheduler-{Group}] 啟動完成，等待 UI 指令…",
                _groupId);

            return Task.CompletedTask;
        }
    }
}
