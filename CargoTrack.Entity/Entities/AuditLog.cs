using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid AppUserId { get; set; }
        public AuditActionType ActionType { get; set; }
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
        public DateTime ActionDate { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Description { get; set; }

        // Navigation Properties
        public virtual AppUser AppUser { get; set; }
    }
}