using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cities;
using CargoTrack.Dto.Dtos.BranchDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class BranchController(IBranchService _branchService, ICityService _cityService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var branches = await _branchService.GetAllAsync();
            return View(branches);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCitiesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBranchDto createBranchDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCitiesAsync();
                return View(createBranchDto);
            }

            await _branchService.CreateAsync(createBranchDto);
            TempData["SuccessMessage"] = "Şube başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var branch = await _branchService.GetByIdAsync(id);
            await PopulateCitiesAsync();
            return View(branch);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateBranchDto updateBranchDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCitiesAsync();
                return View(updateBranchDto);
            }

            await _branchService.UpdateAsync(updateBranchDto);
            TempData["SuccessMessage"] = "Şube bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _branchService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Şube silindi.";
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