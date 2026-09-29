using CargoTrack.Dto.Dtos.AuditLogDtos;

namespace CargoTrack.Business.Services.AuditLogs
{
    public interface IAuditLogService
    {
        Task<List<ResultAuditLogDto>> GetRecentAsync(int count);
    }
}