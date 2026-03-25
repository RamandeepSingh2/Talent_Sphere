using Microsoft.EntityFrameworkCore;
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
            await _context.CareerPlans.Include(c => c.Employee).FirstOrDefaultAsync(c => c.PlanID == id);

        public async Task<IEnumerable<CareerPlan>> GetByEmployeeIdAsync(int employeeId) =>
            await _context.CareerPlans.Where(c => c.EmployeeID == employeeId).ToListAsync();

        public async Task<IEnumerable<CareerPlan>> GetAllAsync() =>
            await _context.CareerPlans.Include(c => c.Employee).ToListAsync();

        public async Task AddAsync(CareerPlan plan) => await _context.CareerPlans.AddAsync(plan);

        public void Update(CareerPlan plan) => _context.CareerPlans.Update(plan);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}