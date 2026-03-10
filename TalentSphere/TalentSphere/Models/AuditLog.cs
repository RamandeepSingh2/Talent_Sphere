using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Models;

namespace TalentSphereAPI.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditID { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        [StringLength(255)]
        public string Action { get; set; }

        [Required]
        [StringLength(255)]
        public string Resource { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
