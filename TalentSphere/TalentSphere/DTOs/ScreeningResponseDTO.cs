using System;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class ScreeningResponseDTO
    {
        public int ScreeningID { get; set; }

        public int ApplicationID { get; set; }

        public int RecruiterID { get; set; }

        public ScreeningResult Result { get; set; }

        public string Notes { get; set; }

        public DateTime Date { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
