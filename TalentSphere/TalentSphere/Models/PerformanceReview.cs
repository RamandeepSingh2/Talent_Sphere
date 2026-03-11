
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentSphere.Models
{
    public class PerformanceReview
    {
        public int ReviewID { get; set; }

        public int EmployeeID { get; set; }

        public int ManagerID { get; set; }

        public decimal Score { get; set; }

        public string Comments { get; set; }

        public DateTime Date { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual User Manager { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}