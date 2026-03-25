using TalentSphere.DTOs.Notification;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDTO> CreateNotificationAsync(CreateNotificationDTO dto);

        
        Task<IEnumerable<NotificationResponseDTO>> GetUserNotificationsAsync(int userId);

        
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        
    }
}
