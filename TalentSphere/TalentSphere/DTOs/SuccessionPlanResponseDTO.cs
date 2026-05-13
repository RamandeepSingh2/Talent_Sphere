// ─────────────────────────────────────────────────────────────────────────────
// FILE PATH: DTOs/SuccessionPlanResponseDTO.cs
// CHANGE: Added TargetPosition and TargetDate fields
// ─────────────────────────────────────────────────────────────────────────────

namespace TalentSphere.DTOs
{
    public class SuccessionPlanResponseDTO
    {
        public int SuccessionID { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public int SuccessorID { get; set; }
        public string? SuccessorName { get; set; }
        public string Status { get; set; } = string.Empty;

        // NEW
        public string? TargetPosition { get; set; }
        public DateTime? TargetDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}