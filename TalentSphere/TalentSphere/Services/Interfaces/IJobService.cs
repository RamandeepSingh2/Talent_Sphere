using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces;

public interface IJobService
{
    Task<Job> CreateJobAsync(CreateJobDTO dto);
    Task<Job> GetByIdAsync(int id);
    Task<List<Job>> GetAllJobsAsync();
    Task<Job> UpdateJobAsync(int id, UpdateJobDTO dto);
    Task<bool> DeleteJobAsync(int id);
}
