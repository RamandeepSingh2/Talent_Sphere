using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Models;

namespace TalentSphere.Models
{
    public class AuditLog
    {
        public int AuditID { get; set; }

        public int UserID { get; set; }

        public virtual User User { get; set; }

        public string Action { get; set; }

        public string Resource { get; set; }

        public DateTime Timestamp { get; set; } 

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; } 

        public Boolean IsDeleted { get; set; }
    }
}
