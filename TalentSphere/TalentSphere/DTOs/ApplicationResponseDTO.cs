using System;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class ApplicationResponseDTO
    {
        public int ApplicationID { get; set; }

        public int JobID { get; set; }

        public int CandidateID { get; set; }

        public DateTime SubmittedDate { get; set; }

        public ApplicationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
