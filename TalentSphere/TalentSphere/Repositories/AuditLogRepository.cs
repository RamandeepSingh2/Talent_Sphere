using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            var entity = (await _context.Set<AuditLog>().AddAsync(auditLog)).Entity;
            return entity;
        }

        public async Task<AuditLog> GetByIdAsync(int id)
        {
            return await _context.Set<AuditLog>().FirstOrDefaultAsync(a => a.AuditID == id && !EF.Property<bool>(a, "IsDeleted"));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.Set<AuditLog>()
                .AsNoTracking()
                .Where(a => !EF.Property<bool>(a, "IsDeleted"))
                .ToListAsync();
        }
    }
}
