using System.ComponentModel.DataAnnotations;
using TalentSphere.Enums;

namespace TalentSphere.DTOs.Notification
{
    public class CreateNotificationDTO
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int EntityID { get; set; } // ID of Job, Review, or Plan

        [Required]
        [MaxLength(200)]
        public string Message { get; set; }

        [Required]
        public NotificationCategory Category { get; set; }


    }
}
