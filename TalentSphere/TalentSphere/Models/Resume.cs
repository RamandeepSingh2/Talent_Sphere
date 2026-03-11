using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class Resume
        {
        public int ResumeID { get; set; }

            public int CandidateID { get; set; }

            public virtual User Candidate { get; set; }

            public string FileURI { get; set; }
            public DateTime UploadedDate { get; set; }

            public ResumeStatus Status { get; set; } 

            public DateTime CreatedAt { get; set; }

            public DateTime? UpdatedAt { get; set; }

            public Boolean IsDeleted { get; set; }
    }
}