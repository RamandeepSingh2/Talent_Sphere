namespace TalentSphere.DTOs.PerformanceReview
{
    public class PerformanceReviewDTO
    {
        public int ReviewID { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public int ManagerID { get; set; }
        public string? ManagerName { get; set; }       // NEW — shows who reviewed
        public int Rating { get; set; }
        public string? Comments { get; set; }
        public string? ReviewPeriod { get; set; }       // NEW — "Q1 2026"
        public string? AreasToImprove { get; set; }     // NEW
        public DateTime ReviewDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
