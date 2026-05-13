// FILE: Models/CareerPlan.cs
// CHANGES: Added TargetRole, TargetDate, ReviewID fields
// Removed Timeline (replaced by TargetDate which is more precise)

using System;
using TalentSphere.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentSphere.Models
{
    public class CareerPlan
    {
        public int PlanID { get; set; }
        public int EmployeeID { get; set; }
        public string Goals { get; set; }

        // NEW: what role/position the employee is aiming for e.g. "Senior Engineer"
        public string? TargetRole { get; set; }

        // NEW: when the employee should achieve the goals
        public DateTime? TargetDate { get; set; }

        // NEW: which performance review triggered this plan (one review = one plan)
        public int? ReviewID { get; set; }
        public virtual PerformanceReview? Review { get; set; }

        public CareerPlanStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
