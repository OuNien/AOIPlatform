using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace UI.GatewayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PanelController : ControllerBase
    {
        private readonly IMessageBus _bus;
        private readonly ResultStore _store;
        private readonly int _groupId;

        public PanelController(
            IMessageBus bus,
            ResultStore store,
            IOptions<UIOptions> options)
        {
            _bus = bus;
            _store = store;
            _groupId = options.Value.GroupId;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartPanel(UiStartPanel request)
        {
            string routingKey = $"aoi.scheduler.{_groupId}";

            await _bus.PublishAsync(request, routingKey);

            return Ok(new { message = "Panel start command sent.", routingKey });
        }

        [HttpGet("{panelId}")]
        public IActionResult GetPanelResult(string panelId)
        {
            var result = _store.Get(panelId);
            if (result == null)
                return NotFound(new { message = "Panel result not ready" });

            return Ok(result);
        }
    }
}
