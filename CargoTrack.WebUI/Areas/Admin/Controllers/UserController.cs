using CargoTrack.Entity.Entities;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class UserController(UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index(string role = "all", string? q = null)
        {
            var allUsers = _userManager.Users.ToList();

            var userRoles = new Dictionary<Guid, string>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "—";
            }

            var filtered = allUsers.Where(u =>
                (role == "all" || userRoles[u.Id] == role) &&
                (string.IsNullOrWhiteSpace(q) ||
                 $"{u.FirstName} {u.LastName} {u.Email}".Contains(q, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(u => u.FirstName)
                .ToList();

            ViewBag.UserRoles = userRoles;
            ViewBag.CurrentRole = role;
            ViewBag.CurrentSearch = q;
            ViewBag.AllCount = allUsers.Count;
            ViewBag.AdminCount = userRoles.Values.Count(r => r == "Admin");
            ViewBag.ManagerCount = userRoles.Values.Count(r => r == "Manager");
            ViewBag.UserCount = userRoles.Values.Count(r => r == "User");

            return View(filtered);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("User"))
            {
                TempData["ErrorMessage"] = "Sadece müşteri hesapları buradan aktif/pasif yapılabilir.";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = user.IsActive ? "Hesap aktifleştirildi." : "Hesap pasifleştirildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}