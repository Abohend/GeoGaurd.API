using GeoGaurd.API.Models.Common;
using GeoGaurd.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoGaurd.API.Controllers;

[ApiController]
[Route("api/logs")]
public sealed class LogsController : ControllerBase
{
    private readonly IBlockedAttemptLogService _blockedAttemptLogService;

    public LogsController(IBlockedAttemptLogService blockedAttemptLogService)
    {
        _blockedAttemptLogService = blockedAttemptLogService;
    }

    [HttpGet("blocked-attempts")]
    public IActionResult GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(ApiResponse.Fail(400, "page and pageSize must be greater than 0."));
        }

        var result = _blockedAttemptLogService.GetPaged(page, pageSize);
        return Ok(ApiResponse.Ok(result));
    }
}
