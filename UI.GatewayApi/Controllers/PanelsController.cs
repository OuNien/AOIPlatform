using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UI.GatewayApi.Dtos;

namespace UI.GatewayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PanelsController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<PanelsController> _logger;

        public PanelsController(IMessageBus messageBus, ILogger<PanelsController> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }

        /// <summary>
        /// 由 UI 呼叫，要求設備開始檢測一片 Panel
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> StartPanel([FromBody] PanelStartRequest request)
        {
            if (request.FieldCount <= 0)
            {
                return BadRequest("FieldCount 必須 > 0");
            }

            var cmd = new UiStartPanel
            {
                PanelId = request.PanelId,
                LotId = request.LotId,
                FieldCount = request.FieldCount,
                RecipeId = request.RecipeId
            };

            _logger.LogInformation(
                "[UI.Gateway] 收到 StartPanel 請求 Panel={Panel}, Lot={Lot}, Fields={Count}, Recipe={Recipe}",
                cmd.PanelId, cmd.LotId, cmd.FieldCount, cmd.RecipeId);

            await _messageBus.PublishAsync(cmd);

            // 簡單回應，說已經接受，實際檢測流程交給後端站別跑
            return Accepted(new
            {
                cmd.PanelId,
                cmd.LotId,
                cmd.FieldCount,
                cmd.RecipeId,
                Status = "Started"
            });
        }
    }
}
