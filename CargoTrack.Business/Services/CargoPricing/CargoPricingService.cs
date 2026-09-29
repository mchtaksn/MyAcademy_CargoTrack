using CargoTrack.DataAccess.Repositories.Branches;
using CargoTrack.DataAccess.Repositories.CargoPrices;
using CargoTrack.DataAccess.Repositories.PricingRules;
using CargoTrack.Entity.Entities;
using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Business.Services.CargoPricing
{
    public class CargoPricingService(
        IPricingRuleRepository _pricingRuleRepository,
        ICargoPriceRepository _cargoPriceRepository,
        IBranchRepository _branchRepository) : ICargoPricingService
    {
        public async Task CalculateAndSaveAsync(Cargo cargo)
        {
            var rule = await _pricingRuleRepository.GetByCargoTypeAsync(cargo.CargoType);
            if (rule is null)
            {
                throw new ValidationException("Bu gönderi tipi için fiyat kuralı tanımlı değil.");
            }

            var originBranch = await _branchRepository.GetByIdAsync(cargo.OriginBranchId);
            var destinationBranch = await _branchRepository.GetByIdAsync(cargo.DestinationBranchId);

            var desi = Math.Round(cargo.Width * cargo.Length * cargo.Height / 3000, 1);
            var unit = (decimal)Math.Max(cargo.Weight, desi);

            var baseAmount = unit * rule.PricePerUnit;
            var cityFee = (originBranch?.CityId == destinationBranch?.CityId)
                ? rule.SameCityFee
                : rule.DifferentCityFee;

            var cargoPrice = new CargoPrice
            {
                CargoId = cargo.Id,
                BaseAmount = baseAmount,
                ServiceTypeAmount = cityFee,
                ExtraServicesAmount = 0,
                TotalAmount = baseAmount + cityFee
            };

            await _cargoPriceRepository.CreateAsync(cargoPrice);
        }
    }
}