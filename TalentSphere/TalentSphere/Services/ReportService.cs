using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
	public class ReportService : IReportService
	{
		private readonly IReportRepository _repository;
		private readonly IMapper _mapper;

		public ReportService(IReportRepository repository , IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}
		public async Task<Report> CreateReportAsync(CreateReportDTO dto)
		{
			var report = _mapper.Map<Report>(dto);
			report.CreatedAt = DateTime.UtcNow;

			var added = await _repository.AddAsync(report);
			await _repository.SaveChangesAsync();

			return added;
		}

		public async Task<Report> GetByIdAsync(int id)
		{
			return await _repository.GetByIdAsync(id);
		}
		public async Task<List<Report>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}
		public async Task<Report> UpdateAsync(int id, UpdateReportDTO dto)
		{
			var report = await _repository.GetByIdAsync(id);
			if (report == null){
				return null;
			}
			report.Scope = dto.Scope;
			report.Metrics = dto.Metrics;
			report.GenerateDate = dto.GenerateDate;

			await _repository.SaveChangesAsync();
			return report;
		}
		public async Task<bool> DeleteAsync(int id)
		{
			var report = await _repository.GetByIdAsync(id);
			if (report == null)
			{
				return false;
			}
			await _repository.DeleteAsync(report);
			await _repository.SaveChangesAsync();
			return true;

		}

	}
}
