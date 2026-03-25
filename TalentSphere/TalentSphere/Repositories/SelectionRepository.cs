using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
    public class SelectionRepository : ISelectionRepository
    {
        private readonly AppDbContext _context;

        public SelectionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Selection> AddAsync(Selection selection)
        {
            var entity = (await _context.Set<Selection>().AddAsync(selection)).Entity;
            return entity;
        }

        public async Task<Selection> GetByIdAsync(int id)
        {
            return await _context.Set<Selection>().FirstOrDefaultAsync(s => s.SelectionID == id && !EF.Property<bool>(s, "IsDeleted")) ?? null!;
        }

        public async Task<List<Selection>> GetAllAsync()
        {
            return await _context.Set<Selection>().Where(s => !EF.Property<bool>(s, "IsDeleted")).ToListAsync();
        }

        public async Task UpdateAsync(Selection selection)
        {
            _context.Set<Selection>().Update(selection);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Selection selection)
        {
            _context.Set<Selection>().Remove(selection);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
