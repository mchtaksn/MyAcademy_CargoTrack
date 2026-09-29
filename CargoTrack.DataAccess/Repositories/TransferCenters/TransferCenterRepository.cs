using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.TransferCenters
{
    public class TransferCenterRepository : GenericRepository<TransferCenter>, ITransferCenterRepository
    {
        private readonly AppDbContext _context;

        public TransferCenterRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<List<TransferCenter>> GetAllAsync()
        {
            return await _context.TransferCenters.Include(t => t.City).ToListAsync();
        }

        public override async Task<TransferCenter> GetByIdAsync(Guid id)
        {
            return await _context.TransferCenters.Include(t => t.City).FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}