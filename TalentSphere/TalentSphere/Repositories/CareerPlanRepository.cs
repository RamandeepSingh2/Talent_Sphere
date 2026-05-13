using Microsoft.EntityFrameworkCore;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Config;

namespace TalentSphere.Repositories
{
    public class CareerPlanRepository : ICareerPlanRepository
    {
        private readonly AppDbContext _context;

        public CareerPlanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CareerPlan?> GetByIdAsync(int id) =>
            await _context.CareerPlans
                .Include(c => c.Employee)
                .Include(c => c.Review)
                .FirstOrDefaultAsync(c => c.PlanID == id);

        public async Task<IEnumerable<CareerPlan>> GetByEmployeeIdAsync(int employeeId) =>
            await _context.CareerPlans
                .Include(c => c.Review)
                .Where(c => c.EmployeeID == employeeId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<CareerPlan>> GetAllAsync() =>
            await _context.CareerPlans
                .Include(c => c.Employee)
                .Include(c => c.Review)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        // NEW: returns the current active/in-progress plan for an employee
        // Used to enforce one-active-plan-per-employee rule
        public async Task<CareerPlan?> GetActiveByEmployeeIdAsync(int employeeId) =>
            await _context.CareerPlans
                .FirstOrDefaultAsync(c =>
                    c.EmployeeID == employeeId &&
                    (c.Status == CareerPlanStatus.Planned || c.Status == CareerPlanStatus.InProgress) &&
                    !c.IsDeleted);

        // NEW: returns the plan already linked to a specific review
        // Used to prevent creating two plans from the same review
        public async Task<CareerPlan?> GetByReviewIdAsync(int reviewId) =>
            await _context.CareerPlans
                .FirstOrDefaultAsync(c => c.ReviewID == reviewId && !c.IsDeleted);

        public async Task AddAsync(CareerPlan plan) =>
            await _context.CareerPlans.AddAsync(plan);

        public void Update(CareerPlan plan) =>
            _context.CareerPlans.Update(plan);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
