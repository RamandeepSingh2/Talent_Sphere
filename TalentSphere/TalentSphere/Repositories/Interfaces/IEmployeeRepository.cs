using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
