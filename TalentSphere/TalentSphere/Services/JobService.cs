using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _repository;
        private readonly IMapper _mapper;

        public JobService(IJobRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Job> CreateJobAsync(CreateJobDTO dto)
        {
            var job = _mapper.Map<Job>(dto);
            job.PostedDate = DateTime.UtcNow;
            job.CreatedAt = DateTime.UtcNow;

            // Ensure default status if not provided
            if (job.Status == 0)
            {
                job.Status = JobStatus.Open;
            }

            var added = await _repository.AddAsync(job);
            await _repository.SaveChangesAsync();
            return added;
        }

        public async Task<Job> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Job> UpdateJobAsync(int id, UpdateJobDTO dto)
        {
            var job = await _repository.GetByIdAsync(id);
            if (job == null)
                return null;

            _mapper.Map(dto, job);
            job.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(job);
            await _repository.SaveChangesAsync();
            return job;
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _repository.GetByIdAsync(id);
            if (job == null)
                return false;

            await _repository.DeleteAsync(job);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
