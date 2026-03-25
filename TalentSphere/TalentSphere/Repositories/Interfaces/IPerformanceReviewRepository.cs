using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IPerformanceReviewRepository
    {
        Task<PerformanceReview> AddAsync(PerformanceReview review);
        Task<PerformanceReview> GetByIdAsync(int id);
        Task<List<PerformanceReview>> GetAllAsync(int? employeeId = null);
        Task<PerformanceReview> UpdateAsync(PerformanceReview review);
        Task SaveChangesAsync();
    }
}
