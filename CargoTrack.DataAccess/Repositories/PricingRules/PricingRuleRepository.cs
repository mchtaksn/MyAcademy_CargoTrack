using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.PricingRules
{
    public class PricingRuleRepository : GenericRepository<PricingRule>, IPricingRuleRepository
    {
        private readonly AppDbContext _context;

        public PricingRuleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PricingRule?> GetByCargoTypeAsync(CargoType cargoType)
        {
            return await _context.PricingRules.FirstOrDefaultAsync(p => p.CargoType == cargoType);
        }
    }
}