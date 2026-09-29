using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Cargos
{
    public interface ICargoRepository : IRepository<Cargo>
    {
        Task<Cargo> GetByTrackCodeAsync(string trackCode);
        Task<List<Cargo>> GetByBranchAsync(Guid branchId);
        Task<List<Cargo>> GetByCustomerAsync(Guid appUserId);
        Task<List<Cargo>> GetDeliveredForTrainingAsync();

        Task<List<Cargo>> GetPendingCargosForRoutingAsync();
        Task<(List<Cargo> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? statusGroup, string? search);
        Task<(int All, int Transit, int Delivered, int Problem)> GetStatusCountsAsync(string? search);
    }
}