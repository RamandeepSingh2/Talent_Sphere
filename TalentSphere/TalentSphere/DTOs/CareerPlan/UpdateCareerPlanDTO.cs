using TalentSphere.Enums;

namespace TalentSphere.DTOs.CareerPlan
{
    public class UpdateCareerPlanDTO
    {
        public string? Goals { get; set; }
        public string? TargetRole { get; set; }         // NEW
        public DateTime? TargetDate { get; set; }       // NEW
        public CareerPlanStatus? Status { get; set; }
    }
}