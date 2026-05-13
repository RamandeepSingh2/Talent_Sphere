using System.ComponentModel.DataAnnotations;

namespace TalentSphere.DTOs.PerformanceReview
{
    public class CreatePerformanceReviewDTO
    {
        [Required]
        public int EmployeeID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comments { get; set; }

        // NEW: review period e.g. "Q1 2026", "Annual 2025"
        [MaxLength(50)]
        public string? ReviewPeriod { get; set; }

        // NEW: areas the employee needs to improve
        [MaxLength(2000)]
        public string? AreasToImprove { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }
    }
}
