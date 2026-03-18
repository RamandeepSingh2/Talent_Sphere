using System.Threading.Tasks;
using TalentSphere.Models;
using TalentSphere.DTOs;

namespace TalentSphere.Services.Interfaces
{
    public interface IScreeningService
    {
        Task<ScreeningResponseDTO> CreateScreeningAsync(CreateScreeningDTO dto);
        Task<ScreeningResponseDTO> GetByIdAsync(int id);
        Task<ScreeningResponseDTO> UpdateScreeningAsync(int id, UpdateScreeningDTO dto);
        Task<IEnumerable<ScreeningResponseDTO>> GetAllAsync();
        Task<bool> DeleteScreeningAsync(int id);
    }
}