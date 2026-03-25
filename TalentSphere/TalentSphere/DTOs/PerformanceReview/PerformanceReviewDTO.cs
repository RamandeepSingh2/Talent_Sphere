namespace TalentSphere.DTOs.PerformanceReview
{
    public class PerformanceReviewDTO
    {
        public int ReviewID { get; set; }
        public int EmployeeID { get; set; }
        public int EmployeeName { get; set; }
        public int ManagerID { get; set; }
        public decimal Score { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public DateTime? UpdatedAt { get; set; } // This is your "last update"
    }
}
