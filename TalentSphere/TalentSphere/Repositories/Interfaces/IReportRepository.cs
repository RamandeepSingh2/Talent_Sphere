using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
	public interface IReportRepository
	{
		Task<Report> AddAsync(Report report);
		Task<Report> GetByIdAsync(int id);
		Task<List<Report>> GetAllAsync();
		Task DeleteAsync(Report report);
		Task SaveChangesAsync();

	}
}
