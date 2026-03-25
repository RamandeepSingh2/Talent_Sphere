namespace TalentSphere.DTOs.CareerPlan
{
    public class CareerPlanResponseDTO
    {
        public int PlanID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } // Flattened from Employee Entity
        public string Goals { get; set; }
        public string Timeline { get; set; }
        public string Status { get; set; } // String representation of Enum
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
