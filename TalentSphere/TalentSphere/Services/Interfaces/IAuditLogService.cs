using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<AuditLogResponseDto> CreateAuditLogAsync(CreateAuditLogDTO dto);
        Task<AuditLogResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<AuditLogResponseDto>> GetAllAsync();
        Task<AuditLogResponseDto> UpdateAuditLogAsync(int id, UpdateAuditLogDTO dto);
        Task<bool> DeleteAuditLogAsync(int id);
    }
}
