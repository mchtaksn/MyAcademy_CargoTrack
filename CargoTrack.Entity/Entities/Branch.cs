using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; }
        public Guid CityId { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public double Latitude { get; set; } = 39.9207;
        public double Longitude { get; set; } = 32.8541;

        // Navigation Properties
        public virtual City City { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Cargo> OriginCargos { get; set; }
        public virtual ICollection<Cargo> DestinationCargos { get; set; }
        public virtual ICollection<CargoMovement> CargoMovements { get; set; }
    }
}