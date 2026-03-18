using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TalentSphere.Config;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Models;

namespace TalentSphere.Repositories
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly AppDbContext _context;

        public ResumeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Resume> AddAsync(Resume resume)
        {
            var entity = (await _context.Set<Resume>().AddAsync(resume)).Entity;
            return entity;
        }

        public async Task<Resume> GetByIdAsync(int id)
        {
            return await _context.Set<Resume>()
                .FirstOrDefaultAsync(r => r.ResumeID == id && !r.IsDeleted);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Resume>> GetAllAsync()
        {
            return await _context.Set<Resume>()
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }
    }
}