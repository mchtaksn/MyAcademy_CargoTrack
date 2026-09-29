using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DataAccess.Repositories.PricingRules
{
    public interface IPricingRuleRepository : IRepository<PricingRule>
    {
        Task<PricingRule?> GetByCargoTypeAsync(CargoType cargoType);
    }
}