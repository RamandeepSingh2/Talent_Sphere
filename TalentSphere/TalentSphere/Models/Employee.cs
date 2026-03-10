using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Models;

namespace TalentSphereAPI.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        [StringLength(100)]
        public string Position { get; set; }

        public DateTime? JoinDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active";
    }
}
