using TalentSphere.Interfaces;
using TalentSphere.Config;
using TalentSphere.Models;

namespace TalentSphere.Repositories
{
    public class ComplianceRecordRepository : IComplianceRecordRepository
    {
        private readonly AppDbContext _context;

        public ComplianceRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ComplianceRecord> AddComplianceRecordAsync(ComplianceRecord record)
        {
            await _context.ComplianceRecords.AddAsync(record);
            await _context.SaveChangesAsync();
            return record;
        }
    }
}