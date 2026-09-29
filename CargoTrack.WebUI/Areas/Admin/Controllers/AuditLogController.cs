using CargoTrack.Business.Services.AuditLogs;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class AuditLogController(IAuditLogService _auditLogService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var logs = await _auditLogService.GetRecentAsync(100);
            return View(logs);
        }
    }
}