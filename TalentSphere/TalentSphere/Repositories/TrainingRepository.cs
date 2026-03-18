using TalentSphere.Config;
using TalentSphere.Models;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
	public class TrainingRepository : ITrainingRepository
	{
		private readonly AppDbContext _context;

		public TrainingRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task<Training> AddAsync(Training training)
		{
			var entity = (await _context.Set<Training>().AddAsync(training)).Entity;
			await _context.SaveChangesAsync();
			return entity;
		}
		public async Task<Training> GetByIdAsync(int id)
		{
			return await _context.
				Set<Training>().FirstOrDefaultAsync(t => t.TrainingID == id && !t.IsDeleted);
		}
		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<List<Training>> GetAllAsync()
		{
			return await _context.Trainings.ToListAsync();
		}
		public async Task DeleteAsync(Training training)
		{
			_context.Trainings.Remove(training);
		}
	}
}
