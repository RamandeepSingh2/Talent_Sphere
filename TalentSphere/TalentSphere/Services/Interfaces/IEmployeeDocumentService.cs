using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface IEmployeeDocumentService
    {
        Task<EmployeeDocumentResponseDto> CreateEmployeeDocumentAsync(CreateEmployeeDocumentDTO dto);
        Task<EmployeeDocumentResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<EmployeeDocumentResponseDto>> GetAllAsync();
        Task<EmployeeDocumentResponseDto> UpdateEmployeeDocumentAsync(int id, UpdateEmployeeDocumentDTO dto);
        Task<bool> DeleteEmployeeDocumentAsync(int id);
    }
}
