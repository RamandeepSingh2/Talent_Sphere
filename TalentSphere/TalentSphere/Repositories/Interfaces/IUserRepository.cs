using System.Threading.Tasks;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces; 
namespace TalentSphere.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
