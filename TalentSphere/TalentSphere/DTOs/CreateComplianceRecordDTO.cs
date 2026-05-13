using System.ComponentModel.DataAnnotations;

namespace TalentSphere.DTOs
{
    public class CreateComplianceRecordDTO
    {
        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public string RecordType { get; set; }

        public string? Description { get; set; }
        public string? Result { get; set; }      // ADD
        public DateTime? Date { get; set; }      // ADD
    }
}
