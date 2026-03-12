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

        public string Timeline { get; set; }

        public CareerPlanStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }

        public virtual Employee Employee { get; set; }
    }
}