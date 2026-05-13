namespace TalentSphere.DTOs.PerformanceReview
{
    public class PerformanceReviewListDTO
    {
        public int ReviewID { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public string? ManagerName { get; set; }
        public int Rating { get; set; }
        public string? ReviewPeriod { get; set; }
        public DateTime ReviewDate { get; set; }
        public string? Comments { get; set; }
        public string? AreasToImprove { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}