namespace TalentSphere.DTOs.CareerPlan
{
    public class CareerPlanResponseDTO
    {
        public int PlanID { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public string Goals { get; set; }
        public string? TargetRole { get; set; }         // NEW
        public DateTime? TargetDate { get; set; }       // NEW
        public int? ReviewID { get; set; }              // NEW — linked review
        public string? ReviewPeriod { get; set; }       // NEW — from linked review
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
