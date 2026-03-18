using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IEmployeeDocumentRepository
    {
        Task<EmployeeDocument> AddAsync(EmployeeDocument doc);
        Task<EmployeeDocument> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeDocument>> GetAllAsync();
        Task SaveChangesAsync();
        Task<bool> EmployeeExistsAsync(int employeeId);
    }
}
