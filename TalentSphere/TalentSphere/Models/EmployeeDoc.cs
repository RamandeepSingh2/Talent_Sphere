using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class EmployeeDocument
    {
        [Key]
        public int DocumentID { get; set; }

        public int EmployeeID { get; set; }

        public EmployeeDocType DocType { get; set; } 

        public string FileURI { get; set; }

        public DateTime? UploadedDate { get; set; }

        public EmployeeDocVerifyStatus VerifyStatus { get; set; } 

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }


        public virtual Employee Employee { get; set; }
    }
}
