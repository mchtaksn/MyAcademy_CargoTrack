using CargoTrack.Dto.Dtos.CargoDtos;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Services.Cargos
{
    public interface ICargoService
    {
        Task<List<ResultCargoDto>> GetAllAsync();
        Task<UpdateCargoDto> GetByIdAsync(Guid id);
        Task<ResultCargoDto> GetDetailAsync(Guid id);
        Task<ResultCargoDto> GetByTrackCodeAsync(string trackCode);
        Task<TrackResultDto> GetPublicTrackResultAsync(string trackCode);

        Task CreateAsync(CreateCargoDto createCargoDto, Guid createdByEmployeeId, Guid createdByAppUserId);
        Task UpdateAsync(UpdateCargoDto updateCargoDto, Guid updatedByAppUserId);
        Task ChangeStatusAsync(Guid cargoId, CargoStatus newStatus, Guid performedByEmployeeId, Guid performedByAppUserId, Guid? branchId, Guid? transferCenterId, string description);

        Task<string> DispatchForDeliveryAsync(Guid cargoId, Guid employeeId, Guid appUserId, Guid branchId);
        Task DeliverAsync(Guid cargoId, string enteredCode, string receivedByName, Guid employeeId, Guid appUserId, Guid branchId);
        Task RecordFailedDeliveryAsync(Guid cargoId, DeliveryExceptionReason reason, Guid employeeId, Guid appUserId, Guid branchId);

        Task<List<ResultCargoDto>> GetByBranchAsync(Guid branchId);
        Task<List<ResultCargoDto>> GetByCustomerAsync(Guid appUserId);
        Task<PagedCargoResultDto> GetPagedAsync(int page, int pageSize, string? statusGroup, string? search);
        List<CargoStatus> GetAllowedNextStatuses(CargoStatus currentStatus);
    }
}