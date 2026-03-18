using System;
using System.ComponentModel.DataAnnotations;

namespace TalentSphere.DTOs
{
    public class CreateAuditLogDTO
    {
        public int UserID { get; set; }

        [Required]
        public string Action { get; set; }

        [Required]
        public string Resource { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
