using System.Threading.Tasks;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services
{
    public interface IInterviewService
    {
        Task<Interview> CreateInterviewAsync(CreateInterviewDTO dto);
        Task<Interview> GetByIdAsync(int id);
    }
}
