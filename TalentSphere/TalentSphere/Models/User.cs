using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Models;

namespace TalentSphereAPI.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public int RoleID { get; set; }

        [ForeignKey("RoleID")]
        public Role Role { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedDateAt { get; set; } = DateTime.UtcNow;
    }
}
