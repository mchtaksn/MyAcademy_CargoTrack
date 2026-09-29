using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Dto.Dtos.CargoDtos
{
    public class CreateCargoDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid OriginBranchId { get; set; }
        public Guid DestinationBranchId { get; set; }
        public double Weight { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public CargoType CargoType { get; set; }
    }
}