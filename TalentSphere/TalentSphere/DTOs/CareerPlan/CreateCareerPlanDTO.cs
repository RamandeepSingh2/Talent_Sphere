using System.ComponentModel.DataAnnotations;
using TalentSphere.Enums;

namespace TalentSphere.DTOs.CareerPlan
{
    public class CreateCareerPlanDTO
    {
        [Required]
        public int EmployeeID { get; set; }

        [Required]
        [MaxLength(500)]
        public string Goals { get; set; }

        // NEW: what position/role they are aiming for
        [MaxLength(200)]
        public string? TargetRole { get; set; }

        // NEW: when they should achieve the goals
        public DateTime? TargetDate { get; set; }

        // NEW: which review triggered this plan
        public int? ReviewID { get; set; }

        [Required]
        public CareerPlanStatus Status { get; set; } = CareerPlanStatus.Planned;
    }
}