using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace GrabWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _bus;
        private readonly GrabWorkerOptions _opt;

        private string StartPanelKey =>
            $"aoi.grabworker.{_opt.GroupId}.{_opt.Side.ToLower()}.{_opt.WorkerId}";

        private string CaptureOrderKey =>
            $"aoi.grabworker.{_opt.GroupId}.{_opt.Side.ToLower()}.{_opt.WorkerId}.order";

        private string ReportKey =>
            $"aoi.grabcontrol.{_opt.GroupId}.captured";

        public Worker(
            ILogger<Worker> logger,
            IMessageBus bus,
            IOptions<GrabWorkerOptions> opt)
        {
            _logger = logger;
            _bus = bus;
            _opt = opt.Value;

            // GrabControl 廣播 GrabStart 給所有取像站
            _bus.SubscribeAsync<GrabStart>(StartPanelKey, HandleGrabStartAsync);

            // GrabControl 針對每一台 Worker 下 CaptureOrder
            _bus.SubscribeAsync<CaptureOrder>(CaptureOrderKey, HandleCaptureOrderAsync);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "[GrabWorker-{Side}{Id}] Ready (Group={Group}) 订阅 StartPanel={StartKey}, CaptureOrder={OrderKey}",
                _opt.Side, _opt.WorkerId, _opt.GroupId, StartPanelKey, CaptureOrderKey);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 收到 GrabStart：記住目前的 PanelId 與預期 Panel 數
        /// </summary>
        private Task HandleGrabStartAsync(GrabStart grabStart)
        {
            _logger.LogInformation(
                "[GrabWorker-{Side}{Id}] StartPanel Panel={Panel}, ExpectedFrames={Frames}",
                _opt.Side, _opt.WorkerId);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 收到控制站下達的 CaptureOrder 才拍照（模擬）
        /// </summary>
        private async Task HandleCaptureOrderAsync(CaptureOrder order)
        {
            var now = DateTimeOffset.Now;

            var image = new ImageCaptured
            {
                PanelId = order.PanelId,
                Side = _opt.Side, // "Top" / "Bottom"
                StationId = $"{_opt.Side}{_opt.WorkerId}",
                CapturedAt = now
            };

            _logger.LogInformation(
                "[GrabWorker-{Side}{Id}] Capture Panel={Panel} → ImageCaptured",
                _opt.Side, _opt.WorkerId, image.PanelId);

            await _bus.PublishAsync(image, ReportKey);
        }
    }
}
