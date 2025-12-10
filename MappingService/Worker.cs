using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace MappingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _messageBus;
        private readonly Random _random = new();

        private readonly int _groupId;
        private readonly string _subscribeKey;

        public Worker(
            ILogger<Worker> logger,
            IMessageBus messageBus,
            IOptions<MappingOptions> options)
        {
            _logger = logger;
            _messageBus = messageBus;

            _groupId = options.Value.GroupId;
            _subscribeKey = $"aoi.mapping.{_groupId}";

            _logger.LogInformation(
                "[Mapping-{Group}] 訂閱 {Key}",
                _groupId, _subscribeKey);

            _messageBus.SubscribeAsync<ImageCaptured>(_subscribeKey, HandleImageCapturedAsync);
        }

        private async Task HandleImageCapturedAsync(ImageCaptured captured)
        {
            _logger.LogInformation(
                "[Mapping-{Group}] 收到影像 Panel={Panel}, Field={Field}, Image={Image}",
                _groupId, captured.PanelId, captured.FieldId, captured.ImageId);

            // 模擬辨識/Recipe對應
            await Task.Delay(_random.Next(100, 300));

            string recipeId = "RCP-DEFAULT";
            int step = _random.Next(1, 4);

            var mapped = new ImageMapped
            {
                PanelId = captured.PanelId,
                FieldId = captured.FieldId,
                ImageId = captured.ImageId,
                RecipeId = recipeId,
                Step = step
            };

            _logger.LogInformation(
                "[Mapping-{Group}] 完成 mapping Panel={Panel} Field={Field} → Recipe={Recipe} Step={Step}",
                _groupId, mapped.PanelId, mapped.FieldId, mapped.RecipeId, mapped.Step);

            // Publish to InspectControl of same group
            string routingKey = $"aoi.inspectcontrol.{_groupId}";

            await _messageBus.PublishAsync(mapped, routingKey);

            _logger.LogInformation(
                "[Mapping-{Group}] 已送出 InspectOrder → {RoutingKey}",
                _groupId, routingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "[Mapping-{Group}] MappingService 啟動完成，等待影像…",
                _groupId);

            return Task.CompletedTask;
        }
    }
}
