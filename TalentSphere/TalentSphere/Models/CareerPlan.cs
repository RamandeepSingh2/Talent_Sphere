
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentSphere.Models
{
    public class CareerPlan
    {
        [Key]
        public int PlanID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public string Goals { get; set; }

        [StringLength(255)]
        public string Timeline { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        // Navigation Property
        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }
    }
}