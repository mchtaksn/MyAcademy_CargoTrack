using CargoTrack.Business.Services.Dashboard;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class DashboardController(IDashboardService _dashboardService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var dashboard = await _dashboardService.GetDashboardAsync();
            return View(dashboard);
        }
    }
}