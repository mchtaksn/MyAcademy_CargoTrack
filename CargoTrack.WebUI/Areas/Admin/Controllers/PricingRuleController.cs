using CargoTrack.Business.Services.PricingRules;
using CargoTrack.Dto.Dtos.PricingRuleDtos;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class PricingRuleController(IPricingRuleService _pricingRuleService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var rules = await _pricingRuleService.GetAllAsync();
            return View(rules);
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var rule = await _pricingRuleService.GetByIdAsync(id);
            return View(rule);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdatePricingRuleDto updatePricingRuleDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updatePricingRuleDto);
            }

            await _pricingRuleService.UpdateAsync(updatePricingRuleDto);
            TempData["SuccessMessage"] = "Fiyat kuralı güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}