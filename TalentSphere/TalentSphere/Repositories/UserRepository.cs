using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;


namespace TalentSphere.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            var entity = (await _context.Set<User>().AddAsync(user)).Entity;
            return entity;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.UserID == id && !EF.Property<bool>(u, "IsDeleted"));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
 
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == email && !EF.Property<bool>(u, "IsDeleted"));
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>()
                .AsNoTracking()
                .Where(u => !EF.Property<bool>(u, "IsDeleted"))
                .ToListAsync();
        }
    }
}
