using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Deliveries
{
    public class DeliveryRepository : GenericRepository<Delivery>, IDeliveryRepository
    {
        private readonly AppDbContext _context;

        public DeliveryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Delivery?> GetLatestByCargoIdAsync(Guid cargoId)
        {
            return await _context.Deliveries
                .Where(d => d.CargoId == cargoId)
                .OrderByDescending(d => d.CreatedDate)
                .FirstOrDefaultAsync();
        }
    }
}