using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.CargoMovements
{
    public interface ICargoMovementRepository : IRepository<CargoMovement>
    {
        Task<List<CargoMovement>> GetByCargoIdAsync(Guid cargoId);
    }
}