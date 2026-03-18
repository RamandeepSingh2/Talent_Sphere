using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
	public interface IEnrollmentRepository
	{
		Task<Enrollment> AddAsync(Enrollment enrollment);
		Task<Enrollment> GetByIdAsync(int id);
				Task SaveChangesAsync();
		Task<List<Enrollment>> GetAllAsync();
		Task UpdateAsync(Enrollment enrollment);
		Task DeleteAsync(Enrollment enrollment);
	}
}
