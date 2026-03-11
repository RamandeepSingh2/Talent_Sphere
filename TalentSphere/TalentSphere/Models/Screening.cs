using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class Screening
    {
        public int ScreeningID { get; set; }

        public int ApplicationID { get; set; }

        public virtual Application Application { get; set; }

        public int RecruiterID { get; set; }

        public virtual User Recruiter { get; set; }

        public ScreeningResult Result { get; set; } 

        public string? Notes { get; set; }

        public DateTime Date { get; set; } 

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Boolean IsDeleted { get; set; }
    }
}