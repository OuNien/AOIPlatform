using AOI.Common.Messages;
using AOI.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UI.GatewayApi.Dtos;

namespace UI.GatewayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GrabController : ControllerBase
    {
        private readonly IMessageBus _bus;
        private readonly ResultStore _store;
        private readonly int _groupId;

        public GrabController(
            IMessageBus bus,
            ResultStore store,
            IOptions<UIOptions> options)
        {
            _bus = bus;
            _store = store;
            _groupId = options.Value.GroupId;
        }

        [HttpPost("GrabStart")]
        public async Task<IActionResult> StartPanel(GrabStart request)
        {
            string routingKey = $"aoi.scheduler.{_groupId}";

            await _bus.PublishAsync(request, routingKey);

            return Ok(new { message = "Panel start command sent.", routingKey });
        }
    }
}
