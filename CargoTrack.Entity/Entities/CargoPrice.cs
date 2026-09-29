using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class CargoPrice : BaseEntity
    {
        public Guid CargoId { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal ServiceTypeAmount { get; set; }
        public decimal ExtraServicesAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation Properties
        public virtual Cargo Cargo { get; set; }
    }
}