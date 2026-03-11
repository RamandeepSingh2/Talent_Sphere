using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class Application
    {
        public int ApplicationID { get; set; }

        public int JobID { get; set; }

        public virtual Job Job { get; set; }

        public int CandidateID { get; set; }

        public virtual User Candidate { get; set; }

        public DateTime SubmittedDate { get; set; }

        public ApplicationStatus Status { get; set; } 

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }

        public Boolean IsDeleted { get; set; }
    }
    
}
