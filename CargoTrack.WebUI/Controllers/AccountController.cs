using CargoTrack.Business.Services.Branches;
using CargoTrack.Entity.Entities;
using CargoTrack.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBranchService _branchService;

        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IBranchService branchService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _branchService = branchService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            await SafePopulateStatsAsync();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                await SafePopulateStatsAsync();
                return View(model);
            }

            try
            {
                // 1. Kullanıcıyı e-posta ile ara
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                {
                    ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                    await SafePopulateStatsAsync();
                    return View(model);
                }

                // 2. Aktiflik durumunu kontrol et
                if (!user.IsActive)
                {
                    ModelState.AddModelError("", "Hesabınız pasifleştirilmiştir. Lütfen destek ekibiyle iletişime geçin.");
                    await SafePopulateStatsAsync();
                    return View(model);
                }

                // 3. Giriş İşlemi (PasswordSignInAsync e-posta değil, UserName bekler)
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin") || roles.Contains("Manager"))
                    {
                        return RedirectToAction("Index", "Cargo", new { area = "Admin" });
                    }

                    return RedirectToAction("Index", "Cargo");
                }

                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login Exception: {ex.Message}");
                ModelState.AddModelError("", "Giriş yapılırken beklenmeyen bir hata oluştu.");
            }

            await SafePopulateStatsAsync();
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            await SafePopulateStatsAsync();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await SafePopulateStatsAsync();
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Cargo");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            await SafePopulateStatsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Track", "Cargo");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Çökmeleri engelleyen güvenli istatistik doldurma metodu
        private async Task SafePopulateStatsAsync()
        {
            try
            {
                var branches = await _branchService.GetAllAsync();
                if (branches != null && branches.Any())
                {
                    ViewBag.BranchCount = branches.Count;
                    ViewBag.CityCount = branches.Select(b => b.City?.Name).Where(n => n is not null).Distinct().Count();
                }
                else
                {
                    ViewBag.BranchCount = 0;
                    ViewBag.CityCount = 0;
                }
            }
            catch
            {
                ViewBag.BranchCount = 0;
                ViewBag.CityCount = 0;
            }
        }
    }
}