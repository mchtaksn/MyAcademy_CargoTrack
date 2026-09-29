using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.DeliveryExceptions
{
    public class DeliveryExceptionRepository : GenericRepository<DeliveryException>, IDeliveryExceptionRepository
    {
        private readonly AppDbContext _context;

        public DeliveryExceptionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> CountByCargoIdAsync(Guid cargoId)
        {
            return await _context.DeliveryExceptions.CountAsync(d => d.CargoId == cargoId);
        }
    }
}