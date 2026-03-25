using TalentSphere.Models;

namespace TalentSphere.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task<Notification> GetByIdAsync(int id);
        Task UpdateAsync(Notification notification);
        Task MarkAllAsReadForUserAsync(int userId);
        Task SaveChangesAsync();
    }
}
