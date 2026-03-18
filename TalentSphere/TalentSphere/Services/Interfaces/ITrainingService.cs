using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
	public interface ITrainingService
	{
		Task<Training> CreateTrainingAsync(CreateTrainingDTO dto);
		Task<Training> GetbyIdAsync(int id);
		Task<List<Training>> GetAllAsync();
		Task<Training> UpdateAsync(int id, UpdateTrainingDTO dto);
		Task<bool> DeleteAsync(int id);
	}
}
