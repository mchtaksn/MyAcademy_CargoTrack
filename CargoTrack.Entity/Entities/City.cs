using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
    }
}