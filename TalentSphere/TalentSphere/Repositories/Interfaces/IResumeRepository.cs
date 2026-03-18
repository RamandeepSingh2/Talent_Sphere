using System.Threading.Tasks;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IResumeRepository
    {
        Task<Resume> AddAsync(Resume resume);
        Task<Resume> GetByIdAsync(int id);
        Task<IEnumerable<Resume>> GetAllAsync();
        Task SaveChangesAsync();
    }
}