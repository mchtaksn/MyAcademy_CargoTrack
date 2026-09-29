using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Employees
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.AppUser)
                .Include(e => e.Branch).ThenInclude(b => b.City)
                .ToListAsync();
        }

        public override async Task<Employee> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.AppUser)
                .Include(e => e.Branch).ThenInclude(b => b.City)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> GetByAppUserIdAsync(Guid appUserId)
        {
            return await _context.Employees
                .Include(e => e.Branch)
                .FirstOrDefaultAsync(e => e.AppUserId == appUserId);
        }
    }
}