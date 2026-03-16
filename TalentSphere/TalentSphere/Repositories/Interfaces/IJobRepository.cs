using System.Threading.Tasks;
using TalentSphere.Models;

namespace TalentSphere.Interfaces
{
    public interface IJobRepository
    {
        Task<Job> AddAsync(Job job);
        Task<Job> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
