using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
	public interface ITrainingRepository
	{
		Task<Training> AddAsync(Training training);
		Task<Training> GetByIdAsync(int id);

		Task SaveChangesAsync();

		Task<List<Training>> GetAllAsync();
		Task DeleteAsync(Training training);

	}
}
