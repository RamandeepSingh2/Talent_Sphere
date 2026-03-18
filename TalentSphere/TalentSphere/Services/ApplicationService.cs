using System;
using System.Threading.Tasks;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repository;
        private readonly IMapper _mapper;
        private readonly IJobRepository _jobRepository;

        public ApplicationService(IApplicationRepository repository, IMapper mapper, IJobRepository jobRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _jobRepository = jobRepository;
        }

        public async Task<ApplicationResponseDTO> CreateApplicationAsync(CreateApplicationDTO dto)
        {
            // Validate referenced Job exists to avoid FK constraint violations
            var job = await _jobRepository.GetByIdAsync(dto.JobID);
            if (job == null)
            {
                return null; // caller (controller) should translate this to a 400 Bad Request
            }

            var application = _mapper.Map<Application>(dto);
            application.CreatedAt = DateTime.UtcNow;
            application.IsDeleted = false;

            var added = await _repository.AddAsync(application);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApplicationResponseDTO>(added);
        }

        public async Task<ApplicationResponseDTO> UpdateApplicationAsync(int id, UpdateApplicationDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return null;

            // Map provided fields onto existing entity
            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return _mapper.Map<ApplicationResponseDTO>(existing);
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<ApplicationResponseDTO> GetByIdAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return null;
            return _mapper.Map<ApplicationResponseDTO>(existing);
        }

        public async Task<IEnumerable<ApplicationResponseDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ApplicationResponseDTO>>(list);
        }
    }
}
