using System;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class UpdateApplicationDTO
    {
        public int JobID { get; set; }

        public int CandidateID { get; set; }

        // Allow SubmittedDate to be null when not updating
        public DateTime? SubmittedDate { get; set; }

        // Make Status nullable so mapping can ignore it when not provided
        public ApplicationStatus? Status { get; set; }
    }
}
