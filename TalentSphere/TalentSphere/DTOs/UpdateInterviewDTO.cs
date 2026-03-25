using System;
using System.ComponentModel.DataAnnotations;

namespace TalentSphere.DTOs
{
    public class UpdateInterviewDTO
    {
        [Required]
        public int ApplicationID { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly Time { get; set; }

        [Required]
        public int InterviewerID { get; set; }
    }
}
