using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;

namespace GrabWorkerService
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
            _stationName = "Grab-1";   // 之後可以改成從設定檔讀，當作站別名稱

            // 只有 InMemory 版才有 Subscribe，未來換 RabbitMQ 時會改這邊
            if (messageBus is RabbitMqMessageBus inMemory)
            {
                //_messageBus.Subscribe<CaptureOrder>(OnMessageAsync);
                _messageBus.Subscribe<CaptureOrder>($"aoi.grabworker.{_stationName}", OnMessageAsync);

            }
        }

        private async Task OnMessageAsync(object msg)
        {
            // 只處理 CaptureOrder，其它訊息忽略
            if (msg is not CaptureOrder order)
                return;

            _logger.LogInformation(
                "[{Station}] 收到取像命令 Panel={Panel}, Field={Field}",
                _stationName, order.PanelId, order.FieldId);

            // 模擬取像時間（例如 200~500ms）
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
                "[{Station}] 完成取像 ImageId={ImageId}, Path={Path}",
                _stationName, captured.ImageId, captured.ImagePath);

            await _messageBus.PublishAsync(captured);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{Station}] GrabWorker 啟動，等待取像命令…", _stationName);
            // 所有事情都靠 OnMessageAsync 被觸發，不需要 while loop
            return Task.CompletedTask;
        }
    }
}
