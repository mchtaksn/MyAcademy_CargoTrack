using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.Business.Services.Employees;
using CargoTrack.Dto.Dtos.CargoDtos;
using CargoTrack.Dto.Dtos.EmployeeDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Controllers
{
    public class CargoController(
        ICargoService _cargoService,
        IEmployeeService _employeeService,
        IBranchService _branchService,
        UserManager<AppUser> _userManager) : Controller
    {
        [AllowAnonymous]
        public IActionResult Track()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Track(string trackCode)
        {
  
            if (string.IsNullOrWhiteSpace(trackCode))
            {
                ModelState.AddModelError("", "Takip numarası boş olamaz.");
                return View();
            }

            try
            {
                var result = await _cargoService.GetPublicTrackResultAsync(trackCode);
                return View("TrackResult", result);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [Authorize(Roles = "Manager,User")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.FirstName = currentUser!.FirstName;

            List<ResultCargoDto> cargos;

            if (User.IsInRole("Manager"))
            {
                var employee = await _employeeService.GetByAppUserIdAsync(currentUser.Id);
                ViewBag.BranchName = employee.BranchName;
                cargos = await _cargoService.GetByBranchAsync(employee.BranchId);
            }
            else
            {
                cargos = await _cargoService.GetByCustomerAsync(currentUser.Id);
            }

            return View(cargos);
        }

        [Authorize(Roles = "Manager,User")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var cargo = await _cargoService.GetDetailAsync(id);
            var currentUserId = Guid.Parse(_userManager.GetUserId(User));

            if (User.IsInRole("Manager"))
            {
                var employee = await _employeeService.GetByAppUserIdAsync(currentUserId);
                if (cargo.OriginBranchId != employee.BranchId && cargo.DestinationBranchId != employee.BranchId)
                {
                    return Forbid();
                }

                ViewBag.ManagerAction = cargo.CargoStatus switch
                {
                    CargoStatus.Received or CargoStatus.InTransferCenter or CargoStatus.DispatchedFromTransferCenter => "simple",
                    CargoStatus.ArrivedAtDeliveryBranch => "dispatch",
                    CargoStatus.OutForDelivery => "delivery",
                    CargoStatus.FailedDelivery => "redispatch",
                    CargoStatus.InReturnProcess => "return",
                    _ => "none"
                };

                if ((string)ViewBag.ManagerAction == "simple")
                {
                    ViewBag.AllowedNextStatuses = _cargoService.GetAllowedNextStatuses(cargo.CargoStatus);
                }
            }
            else
            {
                if (cargo.SenderId != currentUserId && cargo.ReceiverId != currentUserId)
                {
                    return Forbid();
                }
            }

            return View(cargo);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid cargoId, CargoStatus newStatus, string? description)
        {
            var (currentUser, employee, authorized) = await GetManagerContextAsync(cargoId);
            if (!authorized) return Forbid();

            try
            {
                var finalDescription = string.IsNullOrWhiteSpace(description)
                    ? $"Durum '{newStatus}' olarak güncellendi."
                    : description;

                await _cargoService.ChangeStatusAsync(cargoId, newStatus, employee!.Id, currentUser!.Id, employee.BranchId, null, finalDescription);
                TempData["SuccessMessage"] = "Kargo durumu başarıyla güncellendi.";
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Detail), new { id = cargoId });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Dispatch(Guid cargoId)
        {
            var (currentUser, employee, authorized) = await GetManagerContextAsync(cargoId);
            if (!authorized) return Forbid();

            try
            {
                var code = await _cargoService.DispatchForDeliveryAsync(cargoId, employee!.Id, currentUser!.Id, employee.BranchId);
                TempData["SuccessMessage"] = $"Kargo dağıtıma çıktı. Teslimat kodu: {code}";
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Detail), new { id = cargoId });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Deliver(Guid cargoId, string deliveryCode, string receivedByName)
        {
            var (currentUser, employee, authorized) = await GetManagerContextAsync(cargoId);
            if (!authorized) return Forbid();

            try
            {
                await _cargoService.DeliverAsync(cargoId, deliveryCode, receivedByName, employee!.Id, currentUser!.Id, employee.BranchId);
                TempData["SuccessMessage"] = "Kargo başarıyla teslim edildi.";
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Detail), new { id = cargoId });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> ReportFailedDelivery(Guid cargoId, DeliveryExceptionReason reason)
        {
            var (currentUser, employee, authorized) = await GetManagerContextAsync(cargoId);
            if (!authorized) return Forbid();

            try
            {
                await _cargoService.RecordFailedDeliveryAsync(cargoId, reason, employee!.Id, currentUser!.Id, employee.BranchId);
                TempData["SuccessMessage"] = "Başarısız teslimat kaydedildi.";
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Detail), new { id = cargoId });
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var employee = await _employeeService.GetByAppUserIdAsync(currentUser!.Id);

            ViewBag.OriginBranchName = employee.BranchName;
            await PopulateFormListsAsync(employee.BranchId);

            return View(new CreateCargoDto { OriginBranchId = employee.BranchId });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCargoDto createCargoDto)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var employee = await _employeeService.GetByAppUserIdAsync(currentUser!.Id);

            if (!ModelState.IsValid)
            {
                ViewBag.OriginBranchName = employee.BranchName;
                await PopulateFormListsAsync(employee.BranchId);
                return View(createCargoDto);
            }

            createCargoDto.OriginBranchId = employee.BranchId;
            await _cargoService.CreateAsync(createCargoDto, employee.Id, currentUser!.Id);

            TempData["SuccessMessage"] = "Kargo başarıyla oluşturuldu ve takip numarası üretildi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateFormListsAsync(Guid originBranchId)
        {
            var branches = await _branchService.GetAllAsync();
            ViewBag.Branches = branches
                .Where(b => b.Id != originBranchId)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = $"{b.City?.Name} — {b.Name}"
                })
                .ToList();

            var customers = await _userManager.GetUsersInRoleAsync("User");
            ViewBag.Customers = customers
                .OrderBy(c => c.FirstName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.FirstName} {c.LastName}"
                })
                .ToList();
        }

        private async Task<(AppUser? User, ResultEmployeeDto? Employee, bool Authorized)> GetManagerContextAsync(Guid cargoId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var employee = await _employeeService.GetByAppUserIdAsync(currentUser!.Id);
            var cargo = await _cargoService.GetDetailAsync(cargoId);

            var authorized = cargo.OriginBranchId == employee.BranchId || cargo.DestinationBranchId == employee.BranchId;
            return (currentUser, employee, authorized);
        }
    }
}