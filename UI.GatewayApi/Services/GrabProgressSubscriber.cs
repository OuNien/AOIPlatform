using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;
using UI.GatewayApi.Dtos;
using Microsoft.AspNetCore.SignalR;
using UI.GatewayApi.Hubs;
using System.Drawing;
using System.Text.RegularExpressions;


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

        private readonly IHubContext<ProgressHub> _hub;

        public GrabProgressSubscriber(
            ILogger<GrabProgressSubscriber> logger,
            IMessageBus bus,
            GrabProgressStore store,
            IOptions<UIOptions> opt,
            IHubContext<ProgressHub> hub)
        {
            _logger = logger;
            _bus = bus;
            _store = store;
            _hub = hub;
            _groupId = opt.Value.GroupId;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string key = $"aoi.ui.{_groupId}.grabprogress";
            _logger.LogInformation("Subscribe GrabProgressUpdated event at {Key}", key);

            _bus.SubscribeAsync<GrabProgressUpdated>(key, HandleProgressAsync);

            return Task.CompletedTask;
        }

        private async Task HandleProgressAsync(GrabProgressUpdated msg)
        {
            _store.Upsert(msg);

            // ★ 推給所有 UI 客戶端
            await _hub.Clients.All.SendAsync("GrabProgressUpdated", msg);

            _logger.LogInformation(
                "Progress updated: Batch={Batch}, Top {Top}/{TopExp}, Bottom {Btm}/{BtmExp}",
                msg.BatchId,
                msg.TopCompletedPanels, msg.TopExpectedPanels,
                msg.BottomCompletedPanels, msg.BottomExpectedPanels
            );

            string key =$"aoi.mapping.{_groupId}";

            var mapped = new ImageCaptured
            {
                PanelId = msg.TopCurrentPanelId,
                Side = "Top",
                StationId = "1",
                CapturedAt = DateTimeOffset.Now
            };

            _logger.LogInformation("Type={TypeFullName}", mapped.GetType().FullName);


            _logger.LogInformation(
                "{time} GrabDone Push {panelid} To Mapping Station ###{key}###",
                DateTime.Now, msg.TopCurrentPanelId, key);
            await _bus.PublishAsync(mapped, key);


        }

    }
}
