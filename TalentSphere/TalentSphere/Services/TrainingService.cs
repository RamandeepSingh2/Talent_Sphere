using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
	public class TrainingService : ITrainingService
	{
		private readonly ITrainingRepository _repository;
		private readonly IMapper _mapper;

		public TrainingService(ITrainingRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<Training> CreateTrainingAsync(CreateTrainingDTO dto)
		{
			var training = _mapper.Map<Training>(dto);
			training.CreatedAt = DateTime.UtcNow;

			var added = await _repository.AddAsync(training);
			await _repository.SaveChangesAsync();

			return added;
		}
		public async Task<Training> GetbyIdAsync(int id)
		{
			return await _repository.GetByIdAsync(id);

		}
		public async Task<List<Training>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}
		public async Task<Training> UpdateAsync(int id, UpdateTrainingDTO dto){
			var training = await _repository.GetByIdAsync(id);
			if(training == null){
				return null;
			}
			training.Title = dto.Title;
			training.Description = dto.Description;
			training.Duration = dto.Duration;

			await _repository.SaveChangesAsync();

			return training;
		}
		public async Task<bool> DeleteAsync(int id){
			var training = await _repository.GetByIdAsync(id);
			if (training == null)
			{
				return false;
			}
			await _repository.DeleteAsync(training);
			await _repository.SaveChangesAsync();
			return true;
		}
	}
}
