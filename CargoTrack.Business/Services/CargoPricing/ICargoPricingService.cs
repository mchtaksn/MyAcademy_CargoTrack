using CargoTrack.Entity.Entities;

namespace CargoTrack.Business.Services.CargoPricing
{
    public interface ICargoPricingService
    {
        Task CalculateAndSaveAsync(Cargo cargo);
    }
}