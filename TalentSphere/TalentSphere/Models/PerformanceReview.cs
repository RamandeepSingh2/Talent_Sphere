
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentSphere.Models
{
    public class PerformanceReview
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public int ManagerID { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Score { get; set; }

        public string Comments { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Navigation Properties 
        // These allow you to access the related objects directly in your code
        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("ManagerID")]
        public virtual User Manager { get; set; }
    }
}