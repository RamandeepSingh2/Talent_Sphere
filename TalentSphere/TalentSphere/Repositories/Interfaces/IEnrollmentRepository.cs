using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment> AddAsync(Enrollment enrollment);
        Task<Enrollment?> GetByIdAsync(int id);
        Task<List<Enrollment>> GetAllAsync();
        Task<List<Enrollment>> GetByEmployeeIdAsync(int employeeId);
        Task<List<Enrollment>> GetByTrainingIdAsync(int trainingId);
        Task UpdateAsync(Enrollment enrollment);
        Task SaveChangesAsync();
        Task<int> GetActiveCountByTrainingAsync(int trainingId);
        Task<Enrollment?> GetByEmployeeAndTrainingAsync(int employeeId, int trainingId);
    }
}
