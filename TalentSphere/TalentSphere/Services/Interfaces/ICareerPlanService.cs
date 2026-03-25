using TalentSphere.DTOs.CareerPlan;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface ICareerPlanService
    {
        Task<CareerPlanResponseDTO> CreatePlanAsync(CreateCareerPlanDTO dto);
        Task<CareerPlanResponseDTO?> GetPlanByIdAsync(int id);
        Task<IEnumerable<CareerPlanResponseDTO>> GetEmployeePlansAsync(int employeeId);
        Task<bool> UpdatePlanAsync(int id, UpdateCareerPlanDTO dto);
        Task<bool> SoftDeletePlanAsync(int id);
    }
}
