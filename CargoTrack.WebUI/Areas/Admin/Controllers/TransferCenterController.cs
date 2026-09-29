using CargoTrack.Business.Services.Cities;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.Dto.Dtos.TransferCenterDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class TransferCenterController(ITransferCenterService _transferCenterService, ICityService _cityService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var transferCenters = await _transferCenterService.GetAllAsync();
            return View(transferCenters);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCitiesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransferCenterDto createTransferCenterDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCitiesAsync();
                return View(createTransferCenterDto);
            }

            await _transferCenterService.CreateAsync(createTransferCenterDto);
            TempData["SuccessMessage"] = "Transfer merkezi başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var transferCenter = await _transferCenterService.GetByIdAsync(id);
            await PopulateCitiesAsync();
            return View(transferCenter);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateTransferCenterDto updateTransferCenterDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCitiesAsync();
                return View(updateTransferCenterDto);
            }

            await _transferCenterService.UpdateAsync(updateTransferCenterDto);
            TempData["SuccessMessage"] = "Transfer merkezi bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _transferCenterService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Transfer merkezi silindi.";
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCitiesAsync()
        {
            var cities = await _cityService.GetAllAsync();
            ViewBag.Cities = cities
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }
    }
}