using AOI.Common.Domain;
using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace GrabControlService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;
        private readonly int _groupId;
        private readonly int _workerCount;

        public Worker(
            ILogger<Worker> logger,
            IMessageBus messageBus,
            IOptions<GrabControlOptions> options)
        {
            _logger = logger;
            _messageBus = messageBus;

            _groupId = options.Value.GroupId;
            _workerCount = options.Value.WorkerCount;

            string subscribeKey = $"aoi.grabcontrol.{_groupId}";

            _logger.LogInformation("[GrabControl-{Group}] 訂閱 {Key}", _groupId, subscribeKey);

            _messageBus.SubscribeAsync<Panel>(subscribeKey, HandlePanelScheduledAsync);
        }

        /// <summary>
        /// 收到 Panel 排程事件 → 切 Field → 發 CaptureOrder
        /// </summary>
        private async Task HandlePanelScheduledAsync(Panel panel)
        {
            _logger.LogInformation(
                "[GrabControl-{Group}] 收到 Panel 排程 PanelId={PanelId}, FieldCount={Count}",
                _groupId, panel.PanelId, panel.FieldCount);

            int nextWorker = 1;

            for (int i = 1; i <= panel.FieldCount; i++)
            {
                var fieldId = $"F{i:00}";
                var order = new CaptureOrder
                {
                    PanelId = panel.PanelId,
                    FieldId = fieldId
                };

                // 決定要送給哪一台 GrabWorker
                string routingKey = $"aoi.grabworker.{_groupId}.{nextWorker}";

                _logger.LogInformation(
                    "[GrabControl-{Group}] 發出取像命令 → Panel={Panel} Field={Field} → {RoutingKey}",
                    _groupId, order.PanelId, order.FieldId, routingKey);

                await _messageBus.PublishAsync(order, routingKey);

                // 輪詢下一個 worker
                nextWorker++;
                if (nextWorker > _workerCount)
                    nextWorker = 1;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[GrabControl-{Group}] 啟動完成，等待排程事件...", _groupId);
            return Task.CompletedTask;
        }
    }
}
