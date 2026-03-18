using System;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class UpdateEmployeeDTO
    {
        public string Name { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public DateTime? JoinDate { get; set; }

        // Make nullable so it is only updated when provided
        public EmployeeStatus? Status { get; set; }
    }
}
