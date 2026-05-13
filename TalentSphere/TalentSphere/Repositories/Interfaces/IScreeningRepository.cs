using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IScreeningRepository
    {
        Task<Screening> AddAsync(Screening screening);
        Task<Screening?> GetByIdAsync(int id);
        Task<Screening?> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<Screening>> GetAllAsync();
        Task UpdateAsync(Screening screening);
        Task SaveChangesAsync();
        Task<bool> HasPassedScreeningAsync(int applicationId);

    }
}