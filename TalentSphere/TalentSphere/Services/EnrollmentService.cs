using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
	public class EnrollmentService : IEnrollmentService
	{
		private readonly IEnrollmentRepository _repository;
		private readonly IMapper _mapper;
		public EnrollmentService(IEnrollmentRepository repository , IMapper mapper)
		{
			_repository = repository;
				_mapper = mapper;
		}
		public async Task<Enrollment> CreateEnrollmentAsync(CreateEnrollmentDTO dto)
		{
			var Enrollment = _mapper.Map<Enrollment>(dto);
			Enrollment.CreatedAt = DateTime.UtcNow;

			var added = await _repository.AddAsync(Enrollment);
			await _repository.SaveChangesAsync();
			return added;

		}
		public async Task<Enrollment> GetByIdAsync(int id)
		{
			return await _repository.GetByIdAsync(id);
		}
		public async Task<List<Enrollment>> GetAllAsync(){
			return await _repository.GetAllAsync();
		}
		public async Task<Enrollment> UpdateAsync(int id, UpdateEnrollmentDTO dto
		){
			var enrollment = await _repository.GetByIdAsync(id);
			if(enrollment == null){
				return null;
			}
			enrollment.EmployeeID = dto.EmployeeID;
			enrollment.TrainingID = dto.TrainingID;

			await _repository.UpdateAsync(enrollment);
			return enrollment;
		}
		public async Task<bool> DeleteAsync(int id){
			var enrollment = await _repository.GetByIdAsync(id);
			if(enrollment == null){
				return false;
			}
			await _repository.DeleteAsync(enrollment);
			return true;
		}
	}
}
