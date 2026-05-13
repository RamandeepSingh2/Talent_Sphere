using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface ICareerPlanRepository
    {
        Task<CareerPlan?> GetByIdAsync(int id);
        Task<IEnumerable<CareerPlan>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<CareerPlan>> GetAllAsync();

        // NEW: get the current active/planned plan for an employee
        Task<CareerPlan?> GetActiveByEmployeeIdAsync(int employeeId);

        // NEW: get plan linked to a specific review
        Task<CareerPlan?> GetByReviewIdAsync(int reviewId);

        Task AddAsync(CareerPlan plan);
        void Update(CareerPlan plan);
        Task SaveChangesAsync();
    }
}
