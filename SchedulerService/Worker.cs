using AOI.Common.Domain;
using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;

namespace SchedulerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;

        public Worker(ILogger<Worker> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;

            // 讓 Scheduler 可以收到 UiStartPanel
            if (messageBus is RabbitMqMessageBus inMemory)
            {
                //_messageBus.Subscribe<UiStartPanel>(HandleUiStartPanelAsync);
                _messageBus.Subscribe<UiStartPanel>("aoi.scheduler.1", HandleUiStartPanelAsync);

            }
        }

        private async Task OnMessageAsync(object msg)
        {
            if (msg is UiStartPanel uiStart)
            {
                await HandleUiStartPanelAsync(uiStart);
            }
        }

        private async Task HandleUiStartPanelAsync(UiStartPanel uiStart)
        {
            _logger.LogInformation(
                "[Scheduler] 收到 UI 開始指令 PanelId={PanelId}, LotId={LotId}, FieldCount={FieldCount}, Recipe={Recipe}",
                uiStart.PanelId, uiStart.LotId, uiStart.FieldCount, uiStart.RecipeId);

            var panel = new Panel
            {
                PanelId = uiStart.PanelId,
                LotId = uiStart.LotId,
                FieldCount = uiStart.FieldCount,
                RecipeId = uiStart.RecipeId
            };

            _logger.LogInformation("[Scheduler] 建立排程 Panel={PanelId}", panel.PanelId);

            await _messageBus.PublishAsync(panel);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Scheduler] SchedulerService 啟動，等待 UiStartPanel 指令…");
            // 不再自動每 5 秒亂生 panel，全部由 UI 控制
            return Task.CompletedTask;
        }
    }
}
