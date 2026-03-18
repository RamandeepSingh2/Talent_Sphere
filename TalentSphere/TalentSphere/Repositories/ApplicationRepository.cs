using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TalentSphere.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Application> AddAsync(Application application)
        {
            var entity = (await _context.Set<Application>().AddAsync(application)).Entity;
            return entity;
        }

        public async Task<Application> GetByIdAsync(int id)
        {
            return await _context.Set<Application>()
                .FirstOrDefaultAsync(a => a.ApplicationID == id && !a.IsDeleted);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            return await _context.Set<Application>()
                .AsNoTracking()
                .Where(a => !a.IsDeleted)
                .ToListAsync();
        }
    }
}