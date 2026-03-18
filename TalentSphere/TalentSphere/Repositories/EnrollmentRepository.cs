using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
	public class EnrollmentRepository : IEnrollmentRepository
	{
		private readonly AppDbContext _context;
		public EnrollmentRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task<Enrollment> AddAsync(Enrollment enrollment)
		{
			var entity = (await _context.Set<Enrollment>().AddAsync(enrollment)).Entity;
			return entity;

		}
		public async Task<Enrollment> GetByIdAsync(int id)
		{
			return await _context.Set<Enrollment>().FirstOrDefaultAsync(er => er.EnrollmentID == id && !er.IsDeleted);

		}
		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<List<Enrollment>> GetAllAsync()
		{
			return await _context.Enrollments.ToListAsync();
		}
		public async Task UpdateAsync(Enrollment enrollment)
		{
			_context.Enrollments.Update(enrollment);
			await _context.SaveChangesAsync();
		}
		public async Task DeleteAsync(Enrollment enrollment)
		{
			_context.Enrollments.Remove(enrollment);
			await _context.SaveChangesAsync();
		}
	}
}
