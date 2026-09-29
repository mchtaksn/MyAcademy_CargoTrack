using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class Delivery : BaseEntity
    {
        public Guid CargoId { get; set; }
        public string DeliveryCode { get; set; }
        public bool IsCodeVerified { get; set; }
        public string? ReceivedByName { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public Guid EmployeeId { get; set; }

        // Navigation Properties
        public virtual Cargo Cargo { get; set; }
        public virtual Employee Employee { get; set; }
    }
}