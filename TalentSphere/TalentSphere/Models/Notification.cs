
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }

        public int UserID { get; set; }

        public int EntityID { get; set; }

        public string Message { get; set; }

        public NotificationCategory Category { get; set; }

        public NotificationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
        
        public virtual User User { get; set; }
    }
}