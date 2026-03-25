namespace TalentSphere.DTOs.PerformanceReview
{
    public class PerformanceReviewListDTO
    {
        public int ReviewID { get; set; }
        public int EmployeeID { get; set; }
        public decimal Score { get; set; }
        public DateTime Date { get; set; }
        //public string Comments { get; set; } 
    }
}
