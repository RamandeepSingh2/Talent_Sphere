using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
    public class ScreeningRepository : IScreeningRepository
    {
        private readonly AppDbContext _context;

        public ScreeningRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Screening> AddAsync(Screening screening)
        {
            var entity = (await _context.Set<Screening>().AddAsync(screening)).Entity;
            return entity;
        }

        public async Task<Screening> GetByIdAsync(int id)
        {
            return await _context.Set<Screening>()
                .FirstOrDefaultAsync(s => s.ScreeningID == id && !s.IsDeleted);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Screening>> GetAllAsync()
        {
            return await _context.Set<Screening>()
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }
    }
}