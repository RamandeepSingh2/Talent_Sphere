using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface ICareerPlanRepository
    {
        Task<CareerPlan?> GetByIdAsync(int id);
        Task<IEnumerable<CareerPlan>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<CareerPlan>> GetAllAsync();
        Task AddAsync(CareerPlan plan);
        void Update(CareerPlan plan);
        Task SaveChangesAsync();
    }
}
