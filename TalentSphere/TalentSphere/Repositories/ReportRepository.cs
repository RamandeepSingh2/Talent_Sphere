using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
	public class ReportRepository: IReportRepository

	{
		private readonly AppDbContext _context;

		public ReportRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Report> AddAsync(Report report)
		{
			var entity = (await _context.Set<Report>().AddAsync(report)).Entity;
			return entity;
		}

		public async Task<Report> GetByIdAsync(int id)
		{
			return await _context.Set<Report>()
				.FirstOrDefaultAsync(r => r.ReportID == id && !r.IsDeleted);
		}
		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task<List<Report>> GetAllAsync()
		{
			return await _context.Reports.ToListAsync();
		}
		public async Task DeleteAsync(Report report)
		{
			_context.Reports.Remove(report);
		}
	}
}
