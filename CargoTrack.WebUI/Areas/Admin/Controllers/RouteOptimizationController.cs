using CargoTrack.Business.Services.RouteOptimization;
using CargoTrack.DataAccess.Repositories.Cargos;
using CargoTrack.Dto.RouteOptimization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RouteOptimizationController : Controller
    {
        private readonly IRouteOptimizationService _routeOptimizationService;
        private readonly ICargoRepository _cargoRepository;

        public RouteOptimizationController(
            IRouteOptimizationService routeOptimizationService,
            ICargoRepository cargoRepository)
        {
            _routeOptimizationService = routeOptimizationService;
            _cargoRepository = cargoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingCargos()
        {
            try
            {
                var cargos = await _cargoRepository.GetPendingCargosForRoutingAsync();

                if (cargos != null && cargos.Any())
                {
                    var points = cargos.Select(c => new DeliveryPointDto
                    {
                        CargoId = c.Id,
                        TrackingCode = c.TrackCode ?? "Sistem Kargosu",
                        ReceiverName = c.Receiver != null ? $"{c.Receiver.FirstName} {c.Receiver.LastName}" : "Alıcı",
                        Address = c.DestinationBranch != null ? c.DestinationBranch.Address : "Adres Bilgisi Yok",
                        Latitude = c.DestinationBranch?.City?.Latitude ?? (39.9207 + (Math.Abs(c.Id.GetHashCode()) % 50) * 0.002),
                        Longitude = c.DestinationBranch?.City?.Longitude ?? (32.8541 + (Math.Abs(c.Id.GetHashCode()) % 50) * 0.002)
                    }).ToList();

                    return Json(points);
                }
            }
            catch (Exception ex)
            {
                // Veritabanı sorgusunda bir aksama olsa dahi çökmesini engeller
                System.Diagnostics.Debug.WriteLine($"GetPendingCargos Hatası: {ex.Message}");
            }

            // Hata oluşursa veya veritabanında kargo yoksa sunulacak güvenli simülasyon verisi
            var dummyPoints = new List<DeliveryPointDto>
            {
                new DeliveryPointDto { CargoId = Guid.NewGuid(), TrackingCode = "MYC-2026-101", ReceiverName = "Ahmet Yılmaz", Address = "Kızılay Mah. Atatürk Bulvarı No:12/A", Latitude = 39.9198, Longitude = 32.8532 },
                new DeliveryPointDto { CargoId = Guid.NewGuid(), TrackingCode = "MYC-2026-102", ReceiverName = "Selin Doğan", Address = "Tunalı Hilmi Cad. No:45/3", Latitude = 39.9080, Longitude = 32.8601 },
                new DeliveryPointDto { CargoId = Guid.NewGuid(), TrackingCode = "MYC-2026-103", ReceiverName = "Burak Yıldız", Address = "Bahçelievler 7. Cad. No:8", Latitude = 39.9212, Longitude = 32.8225 },
                new DeliveryPointDto { CargoId = Guid.NewGuid(), TrackingCode = "MYC-2026-104", ReceiverName = "Mehmet Demir", Address = "Çukurambar Mah. 1425. Cad. No:10", Latitude = 39.9025, Longitude = 32.8102 },
                new DeliveryPointDto { CargoId = Guid.NewGuid(), TrackingCode = "MYC-2026-105", ReceiverName = "Ayşe Kaya", Address = "Gaziosmanpaşa Mah. Arjantin Cad. No:19", Latitude = 39.8950, Longitude = 32.8650 }
            };

            return Json(dummyPoints);
        }

        [HttpPost]
        public IActionResult OptimizeRoute([FromBody] List<DeliveryPointDto> points)
        {
            if (points == null || !points.Any())
            {
                return BadRequest("Hesaplanacak durak noktası bulunamadı.");
            }

            var startPoint = points.First();
            var result = _routeOptimizationService.OptimizeCourierRoute(startPoint.Latitude, startPoint.Longitude, points);

            return Json(result);
        }
    }
}