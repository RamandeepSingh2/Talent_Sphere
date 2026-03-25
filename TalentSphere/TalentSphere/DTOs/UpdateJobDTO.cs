using System.ComponentModel.DataAnnotations;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class UpdateJobDTO
    {
        [Required]
        public string JobTitle { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Requirements { get; set; }

        public string Responsibilities { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public string Location { get; set; }

        [Required]
        public JobStatus Status { get; set; }

        [Required]
        public int DepartmentID { get; set; }
    }
}
