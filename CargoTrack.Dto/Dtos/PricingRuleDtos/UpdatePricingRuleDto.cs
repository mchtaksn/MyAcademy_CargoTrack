using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Dto.Dtos.PricingRuleDtos
{
    public class UpdatePricingRuleDto
    {
        public Guid Id { get; set; }
        public CargoType CargoType { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal SameCityFee { get; set; }
        public decimal DifferentCityFee { get; set; }
    }
}