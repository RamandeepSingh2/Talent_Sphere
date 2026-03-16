using TalentSphere.DTOs;

namespace TalentSphere.Interfaces
{
    public interface IComplianceRecordService
    {
        Task<CreateComplianceRecordDTO> CreateComplianceRecordAsync(CreateComplianceRecordDTO dto);
    }
}