using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.CargoMovements
{
    public class CargoMovementRepository : GenericRepository<CargoMovement>, ICargoMovementRepository
    {
        private readonly AppDbContext _context;

        public CargoMovementRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CargoMovement>> GetByCargoIdAsync(Guid cargoId)
        {
            return await _context.CargoMovements
                .Include(cm => cm.Branch)
                .Include(cm => cm.TransferCenter)
                .Where(cm => cm.CargoId == cargoId)
                .OrderBy(cm => cm.MovementDate)
                .ToListAsync();
        }
    }
}