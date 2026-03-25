using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IJobRepository
    {
        Task<Job> AddAsync(Job job);
        Task<Job> GetByIdAsync(int id);
        Task<List<Job>> GetAllAsync();
        Task UpdateAsync(Job job);
        Task DeleteAsync(Job job);
        Task SaveChangesAsync();
    }
}
