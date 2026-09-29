using CargoTrack.Dto.Dtos.CityDtos;

namespace CargoTrack.Dto.Dtos.TransferCenterDtos
{
    public class ResultTransferCenterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CityId { get; set; }
        public CityDto City { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}