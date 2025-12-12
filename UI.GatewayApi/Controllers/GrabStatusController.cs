using Microsoft.AspNetCore.Mvc;
using UI.GatewayApi.Dtos;

namespace UI.GatewayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GrabStatusController : ControllerBase
    {
        private readonly GrabProgressStore _store;

        public GrabStatusController(GrabProgressStore store)
        {
            _store = store;
        }

        // 取得全部批次的進度（清單）
        [HttpGet("batches")]
        public IActionResult GetBatches()
        {
            var all = _store.GetAll();
            return Ok(all);
        }

        // 查詢單一批次的進度
        [HttpGet("batches/{batchId}")]
        public IActionResult GetBatch(string batchId)
        {
            var progress = _store.Get(batchId);
            if (progress == null)
                return NotFound();

            return Ok(progress);
        }
    }
}
