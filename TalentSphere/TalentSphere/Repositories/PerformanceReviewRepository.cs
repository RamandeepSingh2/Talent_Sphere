using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TalentSphere.Config;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
    public class PerformanceReviewRepository : IPerformanceReviewRepository
    {
        private readonly AppDbContext _context;

        public PerformanceReviewRepository(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<PerformanceReview> AddAsync(PerformanceReview review)
        {
            var entity = (await _context.Set<PerformanceReview>().AddAsync(review)).Entity;
            return entity;
        }

        
        public async Task<PerformanceReview> GetByIdAsync(int id)
        {
            var res = await _context.Set<PerformanceReview>()
                .Include(p => p.Employee)
                .Include(p => p.Manager)
                .FirstOrDefaultAsync(p => p.ReviewID == id);
            return res;
        }

        public async Task<PerformanceReview> UpdateAsync(PerformanceReview review)
        {
            var entity = _context.Set<PerformanceReview>().Update(review).Entity;
            return await Task.FromResult(entity);
        }


        public async Task<List<PerformanceReview>> GetAllAsync(int? employeeId = null)
        {
            var query = _context.Set<PerformanceReview>()
         .Include(p => p.Employee)
         .Where(p => !p.IsDeleted);

            // Add filter only if employeeId is provided
            if (employeeId.HasValue)
            {
                query = query.Where(p => p.EmployeeID == employeeId.Value);
            }

            return await query
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
