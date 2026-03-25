using System.ComponentModel.DataAnnotations;

namespace TalentSphere.DTOs.CareerPlan
{
    public class CreateCareerPlanDTO
    {
        [Required]
        public int EmployeeID { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Goals { get; set; }

        [Required]
        [MaxLength(500)]
        public string Timeline { get; set; }


    }
}
