
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentSphere.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        public int UserID { get; set; }

        public int EntityID { get; set; }

        public string Message { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Unread";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}