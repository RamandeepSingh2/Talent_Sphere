using System;

namespace TalentSphere.DTOs
{
    public class AuditLogResponseDto
    {
        public int AuditID { get; set; }
        public int UserID { get; set; }
        public string Action { get; set; }
        public string Resource { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
