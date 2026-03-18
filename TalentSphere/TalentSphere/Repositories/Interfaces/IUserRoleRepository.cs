using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<UserRole> AddAsync(UserRole userRole);
        Task<UserRole> GetByIdAsync(int id);
        Task<IEnumerable<UserRole>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
