using System;
using System;

namespace TalentSphere.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string Name { get; set; }

       

        

        public string Email { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
