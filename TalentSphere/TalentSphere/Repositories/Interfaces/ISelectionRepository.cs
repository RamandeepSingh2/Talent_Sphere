using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface ISelectionRepository
    {
        Task<Selection> AddAsync(Selection selection);
        Task<Selection> GetByIdAsync(int id);
        Task<List<Selection>> GetAllAsync();
        Task UpdateAsync(Selection selection);
        Task DeleteAsync(Selection selection);
        Task SaveChangesAsync();
    }
}
