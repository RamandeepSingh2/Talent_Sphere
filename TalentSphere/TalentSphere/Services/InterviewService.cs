using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _repository;
        private readonly IMapper _mapper;

        public InterviewService(IInterviewRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Interview> CreateInterviewAsync(CreateInterviewDTO dto)
        {
            var interview = _mapper.Map<Interview>(dto);
            interview.CreatedAt = DateTime.UtcNow;

            var added = await _repository.AddAsync(interview);
            await _repository.SaveChangesAsync();
            return added;
        }

        public async Task<Interview> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Interview>> GetAllInterviewsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Interview> UpdateInterviewAsync(int id, UpdateInterviewDTO dto)
        {
            var interview = await _repository.GetByIdAsync(id);
            if (interview == null)
                return null;

            _mapper.Map(dto, interview);
            interview.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(interview);
            await _repository.SaveChangesAsync();
            return interview;
        }

        public async Task<bool> DeleteInterviewAsync(int id)
        {
            var interview = await _repository.GetByIdAsync(id);
            if (interview == null)
                return false;

            await _repository.DeleteAsync(interview);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
