using System.Threading.Tasks;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IScreeningRepository
    {
        Task<Screening> AddAsync(Screening screening);
        Task<Screening> GetByIdAsync(int id);
        Task<IEnumerable<Screening>> GetAllAsync();
        Task SaveChangesAsync();
    }
}