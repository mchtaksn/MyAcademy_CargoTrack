using CargoTrack.Dto.Dtos.BranchDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrack.Dto.Dtos.CityDtos
{
    public class ResultCityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<BranchDto> Branches { get; set; }
    }
}
