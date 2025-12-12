using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;
using UI.GatewayApi.Dtos;

namespace UI.GatewayApi.Services
{
    public class GatewayWorker : BackgroundService
    {
        private readonly ILogger<GatewayWorker> _logger;
        private readonly IMessageBus _bus;
        private readonly ResultStore _store;
        private readonly int _groupId;

        public GatewayWorker(
            ILogger<GatewayWorker> logger,
            IMessageBus bus,
            ResultStore store,
            IOptions<UIOptions> options)
        {
            _logger = logger;
            _bus = bus;
            _store = store;

            _groupId = options.Value.GroupId;

            string subscribeKey = $"aoi.gateway.{_groupId}";

            _logger.LogInformation("[UI Gateway-{Group}] 訂閱 {Key}",
                _groupId, subscribeKey);

            _bus.SubscribeAsync<PanelInspectionCompleted>(subscribeKey, HandleCompletedAsync);
        }

        private Task HandleCompletedAsync(PanelInspectionCompleted completed)
        {
            _logger.LogInformation(
                "[UI Gateway-{Group}] 收到 Panel 完成 PanelId={Panel}",
                _groupId, completed.PanelId);

            _store.Save(completed);

            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[UI Gateway-{Group}] 啟動完成", _groupId);
            return Task.CompletedTask;
        }
    }
}
