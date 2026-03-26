using System.Collections.Generic;
using System.Threading.Tasks;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> GetByIdAsync(int id);
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task SaveChangesAsync();
        Task AddAsync(AuditLog auditLog);
    }
}
