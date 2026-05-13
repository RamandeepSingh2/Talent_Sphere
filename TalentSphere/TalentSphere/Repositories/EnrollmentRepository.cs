using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            return (await _context.Set<Enrollment>().AddAsync(enrollment)).Entity;
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Set<Enrollment>()
                .Include(e => e.Training)
                .Include(e => e.Employee).ThenInclude(emp => emp.User)
                .FirstOrDefaultAsync(e => e.EnrollmentID == id && !e.IsDeleted);
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await _context.Set<Enrollment>()
                .Include(e => e.Training)
                .Include(e => e.Employee).ThenInclude(emp => emp.User)
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Set<Enrollment>()
                .Include(e => e.Training)
                .AsNoTracking()
                .Where(e => e.EmployeeID == employeeId && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetByTrainingIdAsync(int trainingId)
        {
            return await _context.Set<Enrollment>()
                .Include(e => e.Employee).ThenInclude(emp => emp.User)
                .AsNoTracking()
                .Where(e => e.TrainingID == trainingId && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetActiveCountByTrainingAsync(int trainingId) =>
             await _context.Set<Enrollment>()
        .CountAsync(e => e.TrainingID == trainingId
                      && e.status != EnrollmentStatus.Cancelled
                      && !e.IsDeleted);

        public async Task<Enrollment?> GetByEmployeeAndTrainingAsync(int employeeId, int trainingId) =>
            await _context.Set<Enrollment>()
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId
                                       && e.TrainingID == trainingId
                                       && !e.IsDeleted);


        public Task UpdateAsync(Enrollment enrollment)
        {
            _context.Set<Enrollment>().Update(enrollment);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
