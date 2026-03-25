using AutoMapper;
using TalentSphere.DTOs.Notification;
using TalentSphere.Models;
using TalentSphere.Enums;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // 1. CREATE - Returns a DTO instead of the raw Entity
        /// <summary>
        /// Creates a new notification using the specified data transfer object and returns a data transfer object
        /// representing the created notification.
        /// </summary>
        /// <param name="dto">The data transfer object containing the details required to create the notification. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a NotificationResponseDTO that
        /// represents the newly created notification.</returns>
        public async Task<NotificationResponseDTO> CreateNotificationAsync(CreateNotificationDTO dto)
        {
            
            var notification = _mapper.Map<Notification>(dto);
            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();
            return _mapper.Map<NotificationResponseDTO>(notification);
        }

        // 2. GET ALL FOR USER - Mapped to DTO List
        /// <summary>
        /// Asynchronously retrieves all notifications for the specified user.
        /// </summary>
        /// <remarks>This method asynchronously fetches notifications from the repository and maps them to
        /// the NotificationResponseDTO format.</remarks>
        /// <param name="userId">The unique identifier of the user whose notifications are to be retrieved. Must be a positive integer.</param>
        /// <returns>An enumerable collection of NotificationResponseDTO objects representing the user's notifications. The
        /// collection will be empty if the user has no notifications.</returns>
        public async Task<IEnumerable<NotificationResponseDTO>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<NotificationResponseDTO>>(notifications);
        }

        // 3. MARK AS READ - Essential logic for a notification module
        /// <summary>
        /// Marks the specified notification as read if it is currently unread.
        /// </summary>
        /// <remarks>If the notification is already marked as read, no changes are made. The method
        /// updates the notification's status to 'Read' and records the current UTC time as the last updated
        /// time.</remarks>
        /// <param name="notificationId">The unique identifier of the notification to be marked as read. Must be a valid notification ID.</param>
        /// <returns>true if the notification was successfully marked as read or was already marked as read; otherwise, false if
        /// the notification does not exist.</returns>
        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _repository.GetByIdAsync(notificationId);

            if (notification == null) return false;

            // Only update if it's currently unread
            if (notification.Status != NotificationStatus.Read)
            {
                notification.Status = NotificationStatus.Read;
                notification.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(notification);
                await _repository.SaveChangesAsync();
            }

            return true;
        }

        //4.Mark all as Read in one click
        /// <summary>
        /// Marks all notifications as read for the specified user asynchronously.
        /// </summary>
        /// <remarks>This method performs the operation in a single batch and saves the changes to the
        /// underlying data store. The user ID provided must refer to a valid user; otherwise, no notifications will be
        /// updated.</remarks>
        /// <param name="userId">The unique identifier of the user whose notifications will be marked as read. Must correspond to an existing
        /// user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> when all
        /// notifications are successfully marked as read.</returns>
        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            await _repository.MarkAllAsReadForUserAsync(userId);
            await _repository.SaveChangesAsync();
            return true;
        }

        
    }
}