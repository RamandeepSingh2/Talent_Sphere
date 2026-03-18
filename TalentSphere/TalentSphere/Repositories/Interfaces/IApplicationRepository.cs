using System.Threading.Tasks;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Application> AddAsync(Application application);
        Task<Application> GetByIdAsync(int id);
        Task<IEnumerable<Application>> GetAllAsync();
        Task SaveChangesAsync();
    }
}