using CargoTrack.Business.Services.CargoDelayPrediction;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area(Area.Admin)]
    [Authorize(Roles = "Admin")]
    public class MachineLearningController(ICargoDelayPredictionService _predictionService) : Controller
    {
        public IActionResult Index()
        {
            ViewBag.IsModelReady = _predictionService.IsModelReady;
            ViewBag.LastTrainingDataCount = _predictionService.LastTrainingDataCount;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Train()
        {
            try
            {
                var count = await _predictionService.TrainAsync();
                TempData["SuccessMessage"] = $"Model {count} kargo verisiyle eğitildi.";
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Predict(double weight, double width, double length, double height, string cargoType, bool sameCity)
        {
            try
            {
                var result = _predictionService.Predict(weight, width, length, height, cargoType, sameCity, DateTime.UtcNow);
                TempData["PredictionResult"] = result.WillBeDelayed ? "Yüksek Risk" : "Düşük Risk";
                TempData["PredictionProbability"] = Math.Round(result.Probability * 100, 1).ToString();
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}