using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;
using UI.GatewayApi.Dtos;

namespace UI.GatewayApi.Services
{
    /// <summary>
    /// 負責訂閱 GrabControl 送出的取像進度事件
    /// </summary>
    public class GrabProgressSubscriber : BackgroundService
    {
        private readonly ILogger<GrabProgressSubscriber> _logger;
        private readonly IMessageBus _bus;
        private readonly GrabProgressStore _store;
        private readonly int _groupId;

        public GrabProgressSubscriber(
            ILogger<GrabProgressSubscriber> logger,
            IMessageBus bus,
            GrabProgressStore store,
            IOptions<UIOptions> opt)
        {
            _logger = logger;
            _bus = bus;
            _store = store;
            _groupId = opt.Value.GroupId;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string key = $"aoi.ui.{_groupId}.grabprogress";
            _logger.LogInformation("Subscribe GrabProgressUpdated event at {Key}", key);

            _bus.SubscribeAsync<GrabProgressUpdated>(key, HandleProgressAsync);

            return Task.CompletedTask;
        }

        private Task HandleProgressAsync(GrabProgressUpdated msg)
        {
            _store.Upsert(msg);

            _logger.LogInformation(
                "Progress updated: Batch={Batch}, Top {Top}/{TopExp}, Bottom {Btm}/{BtmExp}",
                msg.BatchId,
                msg.TopCompletedPanels, msg.TopExpectedPanels,
                msg.BottomCompletedPanels, msg.BottomExpectedPanels
            );

            return Task.CompletedTask;
        }
    }
}
