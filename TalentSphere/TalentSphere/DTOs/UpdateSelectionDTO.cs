using System;
using System.ComponentModel.DataAnnotations;

namespace TalentSphere.DTOs
{
    public class UpdateSelectionDTO
    {
        [Required]
        public int ApplicationID { get; set; }

        [Required]
        public string Decision { get; set; }

        public string Notes { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
