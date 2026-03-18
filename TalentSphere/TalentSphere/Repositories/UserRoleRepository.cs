using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Interfaces;
using TalentSphere.Repositories.Interfaces; 
using TalentSphere.Models;

namespace TalentSphere.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;

        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserRole> AddAsync(UserRole userRole)
        {
            var entity = (await _context.Set<UserRole>().AddAsync(userRole)).Entity;
            return entity;
        }

        public async Task<UserRole> GetByIdAsync(int id)
        {
            return await _context.Set<UserRole>().FirstOrDefaultAsync(ur => ur.UserRoleId == id && !EF.Property<bool>(ur, "IsDeleted"));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _context.Set<UserRole>()
                .AsNoTracking()
                .Where(ur => !EF.Property<bool>(ur, "IsDeleted"))
                .ToListAsync();
        }
    }
}
