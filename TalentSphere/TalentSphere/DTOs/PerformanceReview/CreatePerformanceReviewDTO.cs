using System.ComponentModel.DataAnnotations;

namespace TalentSphere.DTOs.PerformanceReview
{
    public class CreatePerformanceReviewDTO
    {
        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public int ManagerID { get; set; }

        [Required]
        public decimal Score { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comments { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
