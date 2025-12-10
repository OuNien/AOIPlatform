using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;

namespace MappingService
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
            _stationName = "Mapping-1";   // 之後可改成從設定檔讀

            if (messageBus is RabbitMqMessageBus inMemory)
            {
                //_messageBus.Subscribe<ImageCaptured>(HandleImageCapturedAsync);
                _messageBus.Subscribe<ImageCaptured>("aoi.mapping.1", HandleImageCapturedAsync);

            }
        }

        private async Task OnMessageAsync(object msg)
        {
            // 只處理 ImageCaptured，其它訊息先忽略
            if (msg is not ImageCaptured captured)
                return;

            await HandleImageCapturedAsync(captured);
        }

        private async Task HandleImageCapturedAsync(ImageCaptured captured)
        {
            _logger.LogInformation(
                "[{Station}] 收到影像 ImageId={ImageId}, Panel={Panel}, Field={Field}",
                _stationName, captured.ImageId, captured.PanelId, captured.FieldId);

            // 模擬辨識運算時間
            await Task.Delay(_random.Next(100, 300));

            // 簡單決定 Recipe & Step
            var recipeId = "RCP-DEFAULT";     // 之後可以真正查 DB / 設定站輸出的資料
            var step = _random.Next(1, 4);    // 先隨機 1~3 當作行程

            var mapped = new ImageMapped
            {
                PanelId = captured.PanelId,
                FieldId = captured.FieldId,
                ImageId = captured.ImageId,
                RecipeId = recipeId,
                Step = step
            };

            _logger.LogInformation(
                "[{Station}] 完成 mapping ImageId={ImageId} → Recipe={Recipe}, Step={Step}",
                _stationName, mapped.ImageId, mapped.RecipeId, mapped.Step);

            await _messageBus.PublishAsync(mapped);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{Station}] MappingService 啟動，等待影像…", _stationName);
            return Task.CompletedTask; // 全部靠事件觸發
        }
    }
}
