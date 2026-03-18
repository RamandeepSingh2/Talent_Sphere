using TalentSphere.DTOs;

namespace TalentSphere.Services.Interfaces
{
    public interface IAuditService
{
    Task<CreateAuditDTO> CreateAuditAsync(CreateAuditDTO dto);
}
}
