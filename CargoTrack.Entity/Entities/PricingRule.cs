using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    public class PricingRule : BaseEntity
    {
        public CargoType CargoType { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal SameCityFee { get; set; }
        public decimal DifferentCityFee { get; set; }
    }
}