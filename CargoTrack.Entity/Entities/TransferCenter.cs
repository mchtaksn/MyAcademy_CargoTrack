using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class TransferCenter : BaseEntity
    {
        public string Name { get; set; }
        public Guid CityId { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual City City { get; set; }
        public virtual ICollection<CargoMovement> CargoMovements { get; set; }
    }
}