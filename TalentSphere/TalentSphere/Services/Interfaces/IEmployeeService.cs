using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDTO dto);
        Task<Employee> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeResponseDto>> GetAllAsync();
        Task<EmployeeResponseDto> GetByIdDtoAsync(int id);
        Task<EmployeeResponseDto> UpdateEmployeeAsync(int id, UpdateEmployeeDTO dto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
