using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        Task<Interview> AddAsync(Interview interview);
        Task<Interview> GetByIdAsync(int id);
        Task<List<Interview>> GetAllAsync();
        Task UpdateAsync(Interview interview);
        Task DeleteAsync(Interview interview);
        Task SaveChangesAsync();
    }
}
