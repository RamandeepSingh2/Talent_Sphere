using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Models;

namespace TalentSphereAPI.Models
{
    public class EmployeeDoc
    {
        [Key]
        public int DocumentID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [StringLength(100)]
        public string DocType { get; set; }

        [StringLength(500)]
        public string FileURI { get; set; }

        public DateTime? UploadedDate { get; set; }

        [StringLength(50)]
        public string VerifStatus { get; set; } = "Pending";

        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }
    }
}
