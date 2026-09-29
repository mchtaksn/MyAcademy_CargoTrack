using CargoTrack.Business.Services.Cargos;
using CargoTrack.Dto.Dtos.CargoDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class CargoController(ICargoService _cargoService, UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index(int page = 1, string status = "all", string? q = null)
        {
            if (page < 1) page = 1;
            var result = await _cargoService.GetPagedAsync(page, 10, status, q);

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = q;

            return View(result);
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var cargo = await _cargoService.GetByIdAsync(id);
            return View(cargo);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateCargoDto updateCargoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateCargoDto);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            await _cargoService.UpdateAsync(updateCargoDto, currentUser!.Id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var cargo = await _cargoService.GetDetailAsync(id);
            return View(cargo);
        }
    }
}