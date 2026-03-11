using System;
using System.Collections.Generic;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class Role
    {
        public int RoleID { get; set; }

        public RoleName Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; } 
    }
}
