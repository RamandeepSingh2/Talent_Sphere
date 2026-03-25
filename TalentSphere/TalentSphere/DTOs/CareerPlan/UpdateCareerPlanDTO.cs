using System.ComponentModel.DataAnnotations;
using TalentSphere.Enums;

namespace TalentSphere.DTOs.CareerPlan
{
    public class UpdateCareerPlanDTO
    {
        [Required]
        public string Goals { get; set; }
        [Required]
        public string Timeline { get; set; }
        [Required]
        public CareerPlanStatus Status { get; set; }
    }
}
