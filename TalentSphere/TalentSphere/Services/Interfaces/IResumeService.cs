using System.Threading.Tasks;
using TalentSphere.Models;
using TalentSphere.DTOs;

namespace TalentSphere.Services.Interfaces
{
    public interface IResumeService
    {
        Task<ResumeResponseDTO> CreateResumeAsync(CreateResumeDTO dto);
        Task<ResumeResponseDTO> GetByIdAsync(int id);
        Task<ResumeResponseDTO> UpdateResumeAsync(int id, UpdateResumeDTO dto);
        Task<IEnumerable<ResumeResponseDTO>> GetAllAsync();
        Task<bool> DeleteResumeAsync(int id);
    }
}