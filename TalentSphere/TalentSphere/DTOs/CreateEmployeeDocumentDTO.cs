using System;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class CreateEmployeeDocumentDTO
    {
        public int EmployeeID { get; set; }
        public EmployeeDocType DocType { get; set; }
        public DateTime? UploadedDate { get; set; }
    }
}
