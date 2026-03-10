using System;
using System.ComponentModel.DataAnnotations;
 
namespace TalentSphere.Models
{
    public class Job
    {
        [Key]
        public int JobID { get; set; }
 
        [Required]
        public string Title { get; set; }
 
        [Required]
        public string Department { get; set; }
 
        public string Description { get; set; }
 
        public string Requirements { get; set; }
 
        public DateTime PostedDate { get; set; }
 
        public string Status { get; set; } // Open / Closed
    }
}
 
