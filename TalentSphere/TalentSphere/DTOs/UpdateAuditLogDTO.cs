using System;

namespace TalentSphere.DTOs
{
    public class UpdateAuditLogDTO
    {
        public int? UserID { get; set; }
        public string Action { get; set; }
        public string Resource { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
