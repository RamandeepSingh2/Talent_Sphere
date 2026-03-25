using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface IInterviewService
    {
        Task<Interview> CreateInterviewAsync(CreateInterviewDTO dto);
        Task<Interview> GetByIdAsync(int id);
        Task<List<Interview>> GetAllInterviewsAsync();
        Task<Interview> UpdateInterviewAsync(int id, UpdateInterviewDTO dto);
        Task<bool> DeleteInterviewAsync(int id);
    }
}
