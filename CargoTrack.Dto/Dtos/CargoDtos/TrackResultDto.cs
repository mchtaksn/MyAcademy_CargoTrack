using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Dto.Dtos.CargoDtos
{
    public class TrackResultDto
    {
        public string TrackCode { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public string OriginCityName { get; set; }
        public string DestinationCityName { get; set; }
        public double? OriginLat { get; set; }
        public double? OriginLng { get; set; }
        public double? DestinationLat { get; set; }
        public double? DestinationLng { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public List<CargoMovementTimelineDto> Movements { get; set; } = new();
    }

    public class CargoMovementTimelineDto
    {
        public CargoStatus NewStatus { get; set; }
        public DateTime MovementDate { get; set; }
        public string LocationName { get; set; }
        public string Description { get; set; }
    }
}