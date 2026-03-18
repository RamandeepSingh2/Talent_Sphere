using System;
using TalentSphere.Enums;

namespace TalentSphere.DTOs
{
    public class UpdateEmployeeDocumentDTO
    {
        public int? EmployeeID { get; set; }
        // Use FileURI to match EmployeeDocument model
        public string FileURI { get; set; }
        public DateTime? UploadedDate { get; set; }
        public EmployeeDocType? DocType { get; set; }
        public EmployeeDocVerifyStatus? VerifyStatus { get; set; }
    }
}
