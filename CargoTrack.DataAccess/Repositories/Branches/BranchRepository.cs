using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Branches
{
    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        private readonly AppDbContext _context;

        public BranchRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<List<Branch>> GetAllAsync()
        {
            return await _context.Branches.Include(b => b.City).ToListAsync();
        }

        public override async Task<Branch> GetByIdAsync(Guid id)
        {
            return await _context.Branches.Include(b => b.City).FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}