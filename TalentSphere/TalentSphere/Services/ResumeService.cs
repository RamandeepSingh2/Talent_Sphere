using System;
using System.Threading.Tasks;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _repository;
        private readonly IMapper _mapper;

        public ResumeService(IResumeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResumeResponseDTO> CreateResumeAsync(CreateResumeDTO dto)
        {
            var resume = _mapper.Map<Resume>(dto);
            resume.CreatedAt = DateTime.UtcNow;
            resume.IsDeleted = false;

            var added = await _repository.AddAsync(resume);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ResumeResponseDTO>(added);
        }

        public async Task<ResumeResponseDTO> GetByIdAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return null;
            return _mapper.Map<ResumeResponseDTO>(existing);
        }

        public async Task<IEnumerable<ResumeResponseDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ResumeResponseDTO>>(list);
        }

        public async Task<ResumeResponseDTO> UpdateResumeAsync(int id, UpdateResumeDTO dto)
        {
            var resume = await _repository.GetByIdAsync(id);
            if (resume == null)
                return null;

            // Map provided fields onto existing resume. AutoMapper will only overwrite when dto has values.
            _mapper.Map(dto, resume);
            resume.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return _mapper.Map<ResumeResponseDTO>(resume);
        }

        public async Task<bool> DeleteResumeAsync(int id)
        {
            var resume = await _repository.GetByIdAsync(id);
            if (resume == null)
                return false;

            resume.IsDeleted = true;
            resume.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}