using AOI.Common.Domain;
using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace GrabControlService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageBus _bus;
        private readonly GrabControlOptions _opt;

        private readonly string _startPanelKey;
        private readonly string _imageCapturedKey;

        // PanelId -> 狀態
        private readonly Dictionary<int, GrabPanelContext> _panels = new();

        public Worker(
            ILogger<Worker> logger,
            IMessageBus bus,
            IOptions<GrabControlOptions> opt)
        {
            _logger = logger;
            _bus = bus;
            _opt = opt.Value;

            int g = _opt.GroupId;
            _startPanelKey = $"aoi.grabcontrol.{g}.command";
            _imageCapturedKey = $"aoi.grabcontrol.{g}.captured";

            _bus.SubscribeAsync<GrabStart>(_startPanelKey, HandleGrabStartAsync);
            _bus.SubscribeAsync<ImageCaptured>(_imageCapturedKey, HandleImageCapturedAsync);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "[GrabCtrl-{Group}] 啟動，訂閱 StartPanel={StartKey}, ImageCaptured={ImgKey}",
                _opt.GroupId, _startPanelKey, _imageCapturedKey);
        }

        // ─────────────────────────────────────────────
        // GrabStart：初始化 Panel 的狀態，並下第一張 Panel 的 CaptureOrder
        // ─────────────────────────────────────────────

        private async Task HandleGrabStartAsync(GrabStart grabStart)
        {
            int g = _opt.GroupId;

            var ctx = new GrabPanelContext
            {
                GrabPanel = grabStart.GrabPanel
            };

            ctx.Top.ExpectedPanelCount = grabStart.ExpectedPanelCountTop;
            ctx.Top.TotalWorkers = _opt.TopWorkerCount;

            ctx.Bottom.ExpectedPanelCount = grabStart.ExpectedPanelCountBottom;
            ctx.Bottom.TotalWorkers = _opt.BottomWorkerCount;

            _logger.LogInformation(
                "[GrabCtrl-{Group}] StartPanel Panel={Panel}, Recipe={Recipe}, TopPanels={Top}, BtmPanels={Btm}, TopWorkers={TW}, BtmWorkers={BW}",
                g, grabStart.GrabPanel.PanelSideTop.PanelId, grabStart.GrabPanel.PanelSideTop.PartName,
                grabStart.ExpectedPanelCountTop, grabStart.ExpectedPanelCountBottom,
                _opt.TopWorkerCount, _opt.BottomWorkerCount);

            // 廣播 StartPanel 給所有取像站（Top / Bottom）
            for (int i = 1; i <= _opt.TopWorkerCount; i++)
            {
                await _bus.PublishAsync(grabStart, $"aoi.grabworker.{g}.top.{i}");                
            }

            for (int i = 1; i <= _opt.BottomWorkerCount; i++)
            { 
                await _bus.PublishAsync(grabStart, $"aoi.grabworker.{g}.bottom.{i}");                
            }

            await BroadcastCaptureOrderAsync(ctx, "Bottom", grabStart.GrabPanel.PanelSideTop.PanelId);
            await BroadcastCaptureOrderAsync(ctx, "Top", grabStart.GrabPanel.PanelSideTop.PanelId);
        }

        // ─────────────────────────────────────────────
        // ImageCaptured：一台取像站拍到一張資料
        // ─────────────────────────────────────────────

        private async Task HandleImageCapturedAsync(ImageCaptured cap)
        {
            int g = _opt.GroupId;

            if (!_panels.TryGetValue(cap.PanelId, out var ctx))
            {
                _logger.LogWarning(
                    "[GrabCtrl-{Group}] 收到未知 Panel 的 ImageCaptured Panel={Panel}, Side={Side}",
                    g, cap.PanelId, cap.Side);
                return;
            }

            var side = cap.Side == "Top" ? ctx.Top : ctx.Bottom;

            // A. PanelId 由控制站主導：Worker 必須回報「當前或下一個」 frame
            if (cap.PanelId > side.CurrentPanelId + 1)
            {
                _logger.LogError(
                    "[GrabCtrl-{Group}] Panel={Panel} {Side} FrameIndex 跳太多：Current={Cur}, Recv={Recv}",
                    g, cap.PanelId, cap.Side, side.CurrentPanelId, cap.PanelId);
            }

            // 記錄單站最新 PanelId / 時間
            side.StationLastPanelIndex[cap.StationId] = cap.PanelId;
            side.StationLastSeen[cap.StationId] = cap.CapturedAt;

            // 每一個 PanelId 的 WorkersReceived & FrameFirstSeen
            if (!side.WorkersReceived.TryGetValue(cap.PanelId, out var set))
            {
                set = new HashSet<string>();
                side.WorkersReceived[cap.PanelId] = set;
                side.PanelFirstSeen[cap.PanelId] = cap.CapturedAt;
            }

            set.Add(cap.StationId);

            _logger.LogInformation(
                "[GrabCtrl-{Group}] Recv Panel={Panel}, Side={Side}, Station={Station} ({Count}/{Total})",
                g, cap.PanelId, cap.Side, cap.StationId,
                set.Count, side.TotalWorkers);

            // B. 這一面的當前 Panel 是否完成？（所有 Worker 都回報）
            if (cap.PanelId == side.CurrentPanelId &&
                set.Count == side.TotalWorkers)
            {
                side.CompletedPanels++;

                _logger.LogInformation(
                    "[GrabCtrl-{Group}] Side={Side} Frame={Frame} 完成 ({Completed}/{Expected})",
                    g, cap.Side, cap.PanelId, side.CompletedPanels, side.ExpectedPanelCount);

                // 若還有下一張 Panel 要拍，對該面所有 Worker 下 CaptureOrder
                if (side.CompletedPanels < side.ExpectedPanelCount)
                {
                    side.CurrentPanelId += 1;
                    await BroadcastCaptureOrderAsync(ctx, cap.Side, side.CurrentPanelId);
                }
            }
        }

        // ─────────────────────────────────────────────
        // 廣播 CaptureOrder 給該面的所有 Worker
        // ─────────────────────────────────────────────

        private async Task BroadcastCaptureOrderAsync(GrabPanelContext ctx, string sideName, int panelId)
        {
            _panels[panelId] = ctx;

            int g = _opt.GroupId;
            int workerCount;
            SideContext side;

            if (sideName == "Top")
            {
                workerCount = _opt.TopWorkerCount;
                side = ctx.Top;
            }
            else
            {
                workerCount = _opt.BottomWorkerCount;
                side = ctx.Bottom;
            }

            if (workerCount <= 0)
                return;

            var order = new CaptureOrder
            {
                PanelId = panelId,
            };

            for (int i = 1; i <= workerCount; i++)
            {
                var key = $"aoi.grabworker.{g}.{sideName.ToLower()}.{i}.order";
                _logger.LogInformation(
                    "[GrabCtrl-{Group}] Send CaptureOrder Panel={Panel}, Side={Side} → {Key}",
                    g, side.CurrentPanelId, sideName, key);

                await _bus.PublishAsync(order, key);
            }
        }
    }

    // ─────────────────────────────────────────────
    // Context 類別
    // ─────────────────────────────────────────────

    public class GrabPanelContext
    {
        public Panel GrabPanel { get; set; } = new();

        public SideContext Top { get; set; } = new();
        public SideContext Bottom { get; set; } = new();
    }

    public class SideContext
    {
        public int ExpectedPanelCount { get; set; }
        public int TotalWorkers { get; set; }

        public int CurrentPanelId { get; set; } = 1;
        public int CompletedPanels { get; set; } = 0;

        public Dictionary<int, HashSet<string>> WorkersReceived { get; set; } = new();
        public Dictionary<int, DateTimeOffset> PanelFirstSeen { get; set; } = new();

        public Dictionary<string, int> StationLastPanelIndex { get; set; } = new();
        public Dictionary<string, DateTimeOffset> StationLastSeen { get; set; } = new();
    }
}
