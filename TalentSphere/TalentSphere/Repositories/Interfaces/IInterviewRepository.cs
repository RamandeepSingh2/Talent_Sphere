using System.Threading.Tasks;
using TalentSphere.Models;

namespace TalentSphere.Interfaces
{
    public interface IInterviewRepository
    {
        Task<Interview> AddAsync(Interview interview);
        Task<Interview> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
