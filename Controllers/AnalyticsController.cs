using ForasKhadra.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForasKhadra.API.Controllers;

/// <summary>
/// Read-only JSON endpoints the dashboard calls to draw charts.
/// No AI involved here — just numbers straight from the in-memory data.
/// </summary>
[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly DataService _data;

    public AnalyticsController(DataService data) => _data = data;

    [HttpGet("overview")]
    public IActionResult Overview() => Ok(_data.GetOverview());

    [HttpGet("visitors")]
    public IActionResult Visitors() => Ok(_data.GetMonthlyVisitors());

    [HttpGet("by-type")]
    public IActionResult ByType() => Ok(_data.GetByType());

    [HttpGet("engagement")]
    public IActionResult Engagement() => Ok(_data.GetEngagementByPlatform());

    [HttpGet("top-opportunities")]
    public IActionResult TopOpportunities([FromQuery] int take = 8) =>
        Ok(_data.GetTopOpportunities(take));
}
