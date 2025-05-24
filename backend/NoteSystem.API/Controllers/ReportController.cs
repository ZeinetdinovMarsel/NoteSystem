using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteSystem.Core.Interfaces;

namespace NoteSystem.API.Controllers;
[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<FullReportDto>> GetReport()
    {

        var userIdClaim = User.FindFirst("userId");
        var userId = Guid.Parse(userIdClaim.Value);

        var report = await _reportService.GenerateReportAsync(userId);
        return Ok(report);
    }
}
