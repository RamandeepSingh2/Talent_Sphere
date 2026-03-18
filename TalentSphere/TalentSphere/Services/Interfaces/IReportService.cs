using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
	public interface IReportService
	{
		Task<Report> CreateReportAsync(CreateReportDTO dto);
		Task<Report> GetByIdAsync(int id);
		Task <List<Report>> GetAllAsync();
		Task<Report> UpdateAsync(int id, UpdateReportDTO dto);
		Task<bool> DeleteAsync(int id);
	}
}
