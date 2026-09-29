using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CargoTrack.WebUI.Controllers
{
    public class HomeController(
        ILogger<HomeController> _logger,
        IBranchService _branchService,
        ICargoService _cargoService) : Controller
    {
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var branches = await _branchService.GetAllAsync();
            var cargos = await _cargoService.GetAllAsync();

            ViewBag.BranchCount = branches.Count;
            ViewBag.CityCount = branches.Select(b => b.City?.Name).Where(n => n is not null).Distinct().Count();

            var delivered = cargos.Count(c => c.CargoStatus == CargoTrack.Entity.Entities.Enums.CargoStatus.Delivered);
            ViewBag.DeliveryRatePercent = cargos.Count == 0 ? 0 : Math.Round(delivered * 100.0 / cargos.Count, 0);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionFeature?.Error;

            _logger.LogError(exception, "Global hata sayfası tetiklendi: {Path}", exceptionFeature?.Path);

            var isValidationError = exception is System.ComponentModel.DataAnnotations.ValidationException;

            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = isValidationError
                    ? exception!.Message
                    : "Beklenmeyen bir hata oluştu. Ekibimiz bilgilendirildi, lütfen daha sonra tekrar deneyin."
            };

            return View(model);
        }
    }
}