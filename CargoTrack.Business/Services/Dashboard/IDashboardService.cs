using CargoTrack.Dto.Dtos.DashboardDtos;

namespace CargoTrack.Business.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }
}