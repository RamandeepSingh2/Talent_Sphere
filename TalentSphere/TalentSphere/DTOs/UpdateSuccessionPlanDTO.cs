// ─────────────────────────────────────────────────────────────────────────────
// FILE PATH: DTOs/UpdateSuccessionPlanDTO.cs
// CHANGE: Added TargetPosition and TargetDate fields
// NOTE: class name has a typo in the original (UpdateSuccesionPlanDTO) — kept as-is
// ─────────────────────────────────────────────────────────────────────────────

namespace TalentSphere.DTOs
{
    public class UpdateSuccesionPlanDTO
    {
        public int? SuccessorID { get; set; }
        public string? Status { get; set; }

        // NEW
        public string? TargetPosition { get; set; }
        public DateTime? TargetDate { get; set; }
    }
}