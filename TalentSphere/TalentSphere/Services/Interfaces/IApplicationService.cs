using System.Threading.Tasks;
using TalentSphere.Models;
using TalentSphere.DTOs;

namespace TalentSphere.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<ApplicationResponseDTO> CreateApplicationAsync(CreateApplicationDTO dto);
        Task<ApplicationResponseDTO> GetByIdAsync(int id);
        Task<ApplicationResponseDTO> UpdateApplicationAsync(int id, UpdateApplicationDTO dto);
        Task<IEnumerable<ApplicationResponseDTO>> GetAllAsync();
        Task<bool> DeleteApplicationAsync(int id);
    }
}