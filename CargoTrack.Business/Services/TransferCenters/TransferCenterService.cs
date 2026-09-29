using CargoTrack.DataAccess.Repositories.TransferCenters;
using CargoTrack.Dto.Dtos.TransferCenterDtos;
using CargoTrack.Entity.Entities;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Business.Services.TransferCenters
{
    public class TransferCenterService(ITransferCenterRepository _transferCenterRepository) : ITransferCenterService
    {
        public async Task CreateAsync(CreateTransferCenterDto createTransferCenterDto)
        {
            var transferCenter = createTransferCenterDto.Adapt<TransferCenter>();
            await _transferCenterRepository.CreateAsync(transferCenter);
        }

        public async Task DeleteAsync(Guid id)
        {
            var transferCenter = await _transferCenterRepository.GetByIdAsync(id);
            if (transferCenter is null)
            {
                throw new ValidationException("Transfer merkezi bulunamadı.");
            }
            await _transferCenterRepository.DeleteAsync(transferCenter);
        }

        public async Task<List<ResultTransferCenterDto>> GetAllAsync()
        {
            var transferCenters = await _transferCenterRepository.GetAllAsync();
            return transferCenters.Adapt<List<ResultTransferCenterDto>>();
        }

        public async Task<UpdateTransferCenterDto> GetByIdAsync(Guid id)
        {
            var transferCenter = await _transferCenterRepository.GetByIdAsync(id);
            if (transferCenter is null)
            {
                throw new ValidationException("Transfer merkezi bulunamadı.");
            }
            return transferCenter.Adapt<UpdateTransferCenterDto>();
        }

        public async Task UpdateAsync(UpdateTransferCenterDto updateTransferCenterDto)
        {
            var transferCenter = updateTransferCenterDto.Adapt<TransferCenter>();
            await _transferCenterRepository.UpdateAsync(transferCenter);
        }
    }
}