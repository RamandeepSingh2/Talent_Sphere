// ─────────────────────────────────────────────────────────────────────────────
// FILE PATH: DTOs/CreateSuccessionPlanDTO.cs
// CHANGE: Added TargetPosition and TargetDate fields
// ─────────────────────────────────────────────────────────────────────────────

namespace TalentSphere.DTOs
{
    public class CreateSuccessionPlanDTO
    {
        public int EmployeeID { get; set; }
        public int SuccessorID { get; set; }
        public string Status { get; set; } = "Planned";

        // NEW
        public string? TargetPosition { get; set; }
        public DateTime? TargetDate { get; set; }
    }
}
