using CargoTrack.DataAccess.Repositories.AuditLogs;
using CargoTrack.Dto.Dtos.AuditLogDtos;

namespace CargoTrack.Business.Services.AuditLogs
{
    public class AuditLogService(IAuditLogRepository _auditLogRepository) : IAuditLogService
    {
        public async Task<List<ResultAuditLogDto>> GetRecentAsync(int count)
        {
            var logs = await _auditLogRepository.GetRecentAsync(count);

            return logs.Select(l => new ResultAuditLogDto
            {
                Id = l.Id,
                PerformedByName = l.AppUser is not null ? $"{l.AppUser.FirstName} {l.AppUser.LastName}" : "Sistem",
                ActionType = l.ActionType,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                ActionDate = l.ActionDate,
                OldValue = l.OldValue,
                NewValue = l.NewValue,
                Description = l.Description
            }).ToList();
        }
    }
}