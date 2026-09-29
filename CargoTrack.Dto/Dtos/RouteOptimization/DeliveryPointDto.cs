namespace CargoTrack.Dto.RouteOptimization
{
    public class DeliveryPointDto
    {
        public Guid CargoId { get; set; }
        public string TrackingCode { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int VisitOrder { get; set; }
        public double DistanceFromPreviousKm { get; set; }
    }

    public class OptimizedRouteResultDto
    {
        public Guid CourierId { get; set; }
        public double TotalDistanceKm { get; set; }
        public double EstimatedTimeMinutes { get; set; }
        public List<DeliveryPointDto> OrderedPoints { get; set; } = new();
    }
}