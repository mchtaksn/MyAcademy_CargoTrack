using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Dto.Dtos.CargoDtos
{
    public class ResultCargoDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public CargoType CargoType { get; set; }
        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }

        public string SenderFullName { get; set; }
        public string ReceiverFullName { get; set; }
        public string OriginBranchName { get; set; }
        public string DestinationBranchName { get; set; }
        public string OriginCityName { get; set; }
        public string DestinationCityName { get; set; }

        public double? OriginLat { get; set; }
        public double? OriginLng { get; set; }
        public double? DestinationLat { get; set; }
        public double? DestinationLng { get; set; }

        public double Weight { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public double Desi { get; set; }
        public bool IsDelayed { get; set; }

        // Controller'daki sahiplik kontrolü için
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid OriginBranchId { get; set; }
        public Guid DestinationBranchId { get; set; }

        public List<CargoMovementTimelineDto> Movements { get; set; } = new();
    }
}