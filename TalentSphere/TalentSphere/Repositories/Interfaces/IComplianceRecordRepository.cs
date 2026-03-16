using TalentSphere.Models;

namespace TalentSphere.Interfaces
{
    public interface IComplianceRecordRepository
    {
        Task<ComplianceRecord> AddComplianceRecordAsync(ComplianceRecord record);
    }
}