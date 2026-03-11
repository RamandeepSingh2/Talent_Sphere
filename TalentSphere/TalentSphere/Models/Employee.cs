using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public DateTime? JoinDate { get; set; }
        
        public EmployeeStatus Status { get; set; }
        public bool IsDeleted { get; set;  }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
