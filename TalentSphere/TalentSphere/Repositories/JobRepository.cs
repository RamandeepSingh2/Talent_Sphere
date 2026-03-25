using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;

        public JobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Job> AddAsync(Job job)
        {
            var entity = (await _context.Jobs.AddAsync(job)).Entity;
            return entity;
        }

        public async Task<Job> GetByIdAsync(int id)
        {
            return await _context.Jobs.FirstOrDefaultAsync(j => j.JobID == id && !EF.Property<bool>(j, "IsDeleted")) ?? null!;
        }

        public async Task<List<Job>> GetAllAsync()
        {
            return await _context.Jobs.Where(j => !EF.Property<bool>(j, "IsDeleted")).ToListAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            _context.Jobs.Update(job);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Job job)
        {
            _context.Jobs.Remove(job);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
