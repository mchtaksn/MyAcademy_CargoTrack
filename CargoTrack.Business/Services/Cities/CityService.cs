using CargoTrack.DataAccess.Repositories.Cities;
using CargoTrack.Dto.Dtos.CityDtos;
using CargoTrack.Entity.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrack.Business.Services.Cities
{
    public class CityService(ICityRepository _cityRepository) : ICityService
    {
        public async Task CreateAsync(CreateCityDto createCityDto)
        {
            var city = createCityDto.Adapt<City>();
            await _cityRepository.CreateAsync(city);
        }

        public async Task DeleteAsync(Guid id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city is null)
            {
                throw new ValidationException("Şehir Bulunamadı");
            }
            await _cityRepository.DeleteAsync(city);
        }

        public async Task<List<ResultCityDto>> GetAllAsync()
        {
            var city = await _cityRepository.GetAllAsync();
            return city.Adapt<List<ResultCityDto>>();
        }

        public async Task<UpdateCityDto> GetByIdAsync(Guid id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city is null)
            {
                throw new ValidationException("Şehir Bulunamadı");
            }
            return city.Adapt<UpdateCityDto>();
        }

        public async Task UpdateAsync(UpdateCityDto updateCityDto)
        {
            var city = updateCityDto.Adapt<City>();
            await _cityRepository.UpdateAsync(city);
        }
    }
}
