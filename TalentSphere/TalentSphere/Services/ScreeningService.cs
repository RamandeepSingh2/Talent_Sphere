using System;
using System.Threading.Tasks;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    public class ScreeningService : IScreeningService
    {
        private readonly IScreeningRepository _repository;
        private readonly IMapper _mapper;

        public ScreeningService(IScreeningRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ScreeningResponseDTO> CreateScreeningAsync(CreateScreeningDTO dto)
        {
            var screening = _mapper.Map<Screening>(dto);
            screening.CreatedAt = DateTime.UtcNow;
            screening.IsDeleted = false;

            var added = await _repository.AddAsync(screening);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ScreeningResponseDTO>(added);
        }

        public async Task<ScreeningResponseDTO> GetByIdAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return null;
            return _mapper.Map<ScreeningResponseDTO>(existing);
        }

        public async Task<IEnumerable<ScreeningResponseDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ScreeningResponseDTO>>(list);
        }

        public async Task<ScreeningResponseDTO> UpdateScreeningAsync(int id, UpdateScreeningDTO dto)
        {
            var screening = await _repository.GetByIdAsync(id);
            if (screening == null)
                return null;

            _mapper.Map(dto, screening);
            screening.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return _mapper.Map<ScreeningResponseDTO>(screening);
        }

        public async Task<bool> DeleteScreeningAsync(int id)
        {
            var screening = await _repository.GetByIdAsync(id);
            if (screening == null)
                return false;

            screening.IsDeleted = true;
            screening.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}