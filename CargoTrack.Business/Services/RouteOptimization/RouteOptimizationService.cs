using CargoTrack.Dto.RouteOptimization;

namespace CargoTrack.Business.Services.RouteOptimization
{
    public interface IRouteOptimizationService
    {
        OptimizedRouteResultDto OptimizeCourierRoute(double startLat, double startLng, List<DeliveryPointDto> points);
    }

    public class RouteOptimizationService : IRouteOptimizationService
    {
        private const double EarthRadiusKm = 6371.0;
        private const double AverageCourierSpeedKmH = 30.0; // Şehir içi ortalama hız

        public OptimizedRouteResultDto OptimizeCourierRoute(double startLat, double startLng, List<DeliveryPointDto> points)
        {
            var unvisited = new List<DeliveryPointDto>(points);
            var route = new List<DeliveryPointDto>();

            double currentLat = startLat;
            double currentLng = startLng;
            double totalDistance = 0;
            int order = 1;

            while (unvisited.Count > 0)
            {
                DeliveryPointDto? nearest = null;
                double minDistance = double.MaxValue;

                foreach (var point in unvisited)
                {
                    double dist = CalculateHaversineDistance(currentLat, currentLng, point.Latitude, point.Longitude);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearest = point;
                    }
                }

                if (nearest != null)
                {
                    nearest.VisitOrder = order++;
                    nearest.DistanceFromPreviousKm = Math.Round(minDistance, 2);
                    totalDistance += minDistance;

                    currentLat = nearest.Latitude;
                    currentLng = nearest.Longitude;

                    route.Add(nearest);
                    unvisited.Remove(nearest);
                }
            }

            double totalDistanceKm = Math.Round(totalDistance, 2);
            double estimatedTimeMinutes = Math.Round((totalDistanceKm / AverageCourierSpeedKmH) * 60 + (route.Count * 5), 0); // Her teslimat 5 dk

            return new OptimizedRouteResultDto
            {
                TotalDistanceKm = totalDistanceKm,
                EstimatedTimeMinutes = estimatedTimeMinutes,
                OrderedPoints = route
            };
        }

        private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private static double ToRadians(double angle) => (Math.PI / 180) * angle;
    }
}