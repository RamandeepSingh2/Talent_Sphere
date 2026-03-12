using System;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;
 
namespace TalentSphere.Models
{
    public class Audit
    {
        public int AuditID { get; set; }

        public int HRID { get; set; } // Reference to the HR professional

        public virtual User HR { get; set; }
    
        public string Scope { get; set; }
    
        public string Findings { get; set; }

        public DateTime Date { get; set; }

        public AuditStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }

    }
}