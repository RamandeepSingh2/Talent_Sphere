using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace TalentSphere.Models
{
    public class Selection
    {
        [Key]
        public int SelectionID { get; set; }
 
        [Required]
        public int ApplicationID { get; set; }
 
        [Required]
        public string Decision { get; set; }   // Selected / Rejected
 
        public string Notes { get; set; }
 
        public DateTime Date { get; set; }
 
        // Navigation Property
        [ForeignKey("ApplicationID")]
        public Application Application { get; set; }
    }
}