using CargoTrack.Dto.Dtos.CargoDtos;
using CargoTrack.Dto.Dtos.CityDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrack.Dto.Dtos.BranchDtos
{
    public class ResultBranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CityId { get; set; }
        public CityDto City { get; set; }

        public IList<CargoDto> OriginCargos { get; set; }
        public IList<CargoDto> DestinationCargos { get; set; }
    }
}
