using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
	public interface IEnrollmentService
	{
		Task<Enrollment> CreateEnrollmentAsync(CreateEnrollmentDTO dto);
		Task<Enrollment> GetByIdAsync(int id);

		Task<List<Enrollment>> GetAllAsync();

		Task<Enrollment> UpdateAsync(int id, UpdateEnrollmentDTO dto);

		Task<bool> DeleteAsync(int id);
	}
}
