using System;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Phone { get; set; }

        public UserStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}
