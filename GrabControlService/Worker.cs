using AOI.Common.Domain;
using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;

namespace GrabControlService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;

        public Worker(ILogger<Worker> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;

            // 訂閱 Panel 類別的事件
            if (messageBus is RabbitMqMessageBus inMemory)
            {
                //_messageBus.Subscribe<Panel>(HandlePanelScheduledAsync);
                _messageBus.SubscribeAsync<Panel>("aoi.grabcontrol.1", HandlePanelScheduledAsync);

            }
        }

        private async Task OnMessageReceived(object msg)
        {
            if (msg is Panel panel)
            {
                await HandlePanelScheduledAsync(panel);
            }
        }

        /// <summary>
        /// 收到 Panel 排程事件 → 切 Field → 發 CaptureOrder
        /// </summary>
        private async Task HandlePanelScheduledAsync(Panel panel)
        {
            _logger.LogInformation(
                "[GrabControl] 收到 Panel 排程事件 PanelId={PanelId}, FieldCount={Count}",
                panel.PanelId, panel.FieldCount);

            for (int i = 1; i <= panel.FieldCount; i++)
            {
                var fieldId = $"F{i:00}";
                var order = new CaptureOrder
                {
                    PanelId = panel.PanelId,
                    FieldId = fieldId
                };

                _logger.LogInformation("[GrabControl] 發出取像命令 → Panel={Panel} Field={Field}",
                    order.PanelId, order.FieldId);

                await _messageBus.PublishAsync(order);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[GrabControl] 啟動完成，等待排程事件...");
            return Task.CompletedTask; // GrabControl 不需要 loop，由事件觸發
        }
    }
}
