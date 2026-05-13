// FILE PATH: Models/SuccessionPlan.cs
// CHANGE: Added TargetPosition (string?) and TargetDate (DateTime?) fields

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class SuccessionPlan
    {
        public int SuccessionID { get; set; }
        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
        public int SuccessorID { get; set; }
        public virtual Employee? Successor { get; set; }
        public SuccessionStatus Status { get; set; }

        // NEW: what position/role is this succession plan for
        public string? TargetPosition { get; set; }

        // NEW: when is the successor expected to be ready
        public DateTime? TargetDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
