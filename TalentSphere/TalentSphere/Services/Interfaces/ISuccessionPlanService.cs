using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
	public interface ISuccessionPlanService
	{
		Task<SuccessionPlan> CreateSuccessionPlanAsync(CreateSuccessionPlanDTO dto);
		Task<SuccessionPlan> GetByIdAsync(int id);
		Task<List<SuccessionPlan>> GetAllAsync();
		Task<bool> DeleteAsync(int id);
		Task<SuccessionPlan> UpdateAsync(int id, UpdateSuccesionPlanDTO dto);


	}
}
