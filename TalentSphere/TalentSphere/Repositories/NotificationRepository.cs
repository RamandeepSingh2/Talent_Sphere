using Microsoft.EntityFrameworkCore;
using TalentSphere.Config; // Ensure this points to your DbContext namespace
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. ADD - Returns the entity so you can get the generated ID if needed
        public async Task AddAsync(Notification notification)
        {
            await _context.Set<Notification>().AddAsync(notification);
        }

        // 2. GET BY ID - Includes the User for detail views
        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Set<Notification>()
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.NotificationID == id);
        }

        // 3. GET BY USER ID - Added AsNoTracking for faster performance
        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<Notification>()
                .AsNoTracking() // Faster for read-only lists
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // 4. UPDATE - needed for Status changes
        public async Task UpdateAsync(Notification notification)
        {
            _context.Set<Notification>().Update(notification);
            await Task.CompletedTask; // Since EF Update isn't async, we keep the signature clean
        }


        public async Task MarkAllAsReadForUserAsync(int userId)
        {
            // Fetch only notifications that are currently Unread for this user
            var unreadNotifications = await _context.Set<Notification>()
                .Where(n => n.UserID == userId && n.Status == NotificationStatus.Unread)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.Status = NotificationStatus.Read;
                notification.UpdatedAt = DateTime.UtcNow;
            }
            // Note: We don't call SaveChanges here; the Service will handle the Unit of Work.
        }

        // 5. SAVE CHANGES
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}