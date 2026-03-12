using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;
 
namespace TalentSphere.Models
{
   public class Interview
    {
        public int InterviewID { get; set; }

        public int ApplicationID { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public int InterviewerID { get; set; }

        public InterviewStatus Status { get; set; }

        public Application Application { get; set; }

        public User Interviewer { get; set; }

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; } 
        
        public bool IsDeleted { get; set; }
    }
}
 