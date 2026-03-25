namespace TalentSphere.DTOs.Notification
{
    public class NotificationResponseDTO
    {
        public int NotificationID { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int EntityID { get; set; }
    }
}
