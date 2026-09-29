using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Dto.Dtos.CargoDtos
{
    public class UpdateCargoDto
    {
        public Guid Id { get; set; }
        public double Weight { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public CargoType CargoType { get; set; }
    }
}