using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Employees;
using CargoTrack.Dto.Dtos.EmployeeDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.WebUI.Consts;
using CargoTrack.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class EmployeeController(
        IEmployeeService _employeeService,
        IBranchService _branchService,
        UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllAsync();
            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateBranchesAsync();
            return View(new CreateEmployeeViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateBranchesAsync();
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "Bu e-posta adresi zaten kullanılıyor.");
                await PopulateBranchesAsync();
                return View(model);
            }

            var appUser = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            var identityResult = await _userManager.CreateAsync(appUser, model.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                await PopulateBranchesAsync();
                return View(model);
            }

            await _userManager.AddToRoleAsync(appUser, "Manager");

            await _employeeService.CreateAsync(new CreateEmployeeDto
            {
                AppUserId = appUser.Id,
                BranchId = model.BranchId,
                Position = model.Position,
                IsActive = true
            });

            TempData["SuccessMessage"] = "Personel ve giriş hesabı başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            await PopulateBranchesAsync();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateBranchesAsync();
                return View(updateEmployeeDto);
            }

            await _employeeService.UpdateAsync(updateEmployeeDto);
            TempData["SuccessMessage"] = "Personel bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _employeeService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Personel kaydı silindi.";
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateBranchesAsync()
        {
            var branches = await _branchService.GetAllAsync();
            ViewBag.Branches = branches
                .OrderBy(b => b.City?.Name).ThenBy(b => b.Name)
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = $"{b.City?.Name} — {b.Name}" })
                .ToList();
        }
    }
}