using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
	public interface ISuccessionPlanRepository
	{
		Task<SuccessionPlan> AddAsync(SuccessionPlan successionPlan);
		Task<SuccessionPlan> GetByIdAsync(int id);
		Task<List<SuccessionPlan>> GetAllAsync();
		Task UpdateAsync(SuccessionPlan succession);
		Task DeleteAsync(SuccessionPlan succession);
		Task SaveChangesAsync();
	}
}
