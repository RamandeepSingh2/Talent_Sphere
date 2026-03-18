using System;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class ResumeResponseDTO
    {
        public int ResumeID { get; set; }

        public int CandidateID { get; set; }

        public string FileURI { get; set; }

        public DateTime UploadedDate { get; set; }

        public ResumeStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
