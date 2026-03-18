using System;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class UpdateScreeningDTO
    {
        public int ApplicationID { get; set; }
        public int RecruiterID { get; set; }
        public ScreeningResult? Result { get; set; }
        public string? Notes { get; set; }
        public DateTime? Date { get; set; }
    }
}
