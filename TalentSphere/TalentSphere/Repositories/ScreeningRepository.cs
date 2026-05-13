using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Enums;
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
            return (await _context.Set<Screening>().AddAsync(screening)).Entity;
        }

        public async Task<Screening?> GetByIdAsync(int id)
        {
            return await _context.Set<Screening>()
                .Include(s => s.Application).ThenInclude(a => a.Candidate)
                .Include(s => s.Application).ThenInclude(a => a.Job)
                .Include(s => s.Recruiter)
                .FirstOrDefaultAsync(s => s.ScreeningID == id && !s.IsDeleted);
        }

        public async Task<Screening?> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Set<Screening>()
                .Include(s => s.Application).ThenInclude(a => a.Candidate)
                .Include(s => s.Application).ThenInclude(a => a.Job)
                .Include(s => s.Recruiter)
                .FirstOrDefaultAsync(s => s.ApplicationID == applicationId && !s.IsDeleted);
        }

        public async Task<IEnumerable<Screening>> GetAllAsync()
        {
            return await _context.Set<Screening>()
                .Include(s => s.Application).ThenInclude(a => a.Candidate)
                .Include(s => s.Application).ThenInclude(a => a.Job)
                .Include(s => s.Recruiter)
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        public Task UpdateAsync(Screening screening)
        {
            _context.Set<Screening>().Update(screening);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasPassedScreeningAsync(int applicationId)
        {
            return await _context.Screenings
                .AnyAsync(s => s.ApplicationID == applicationId
                            && s.Result == ScreeningResult.Pass
                            && !s.IsDeleted);
        }

    }
}
