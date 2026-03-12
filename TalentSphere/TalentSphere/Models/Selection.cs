using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;
 
namespace TalentSphere.Models
{
    public class Selection
    {
        public int SelectionID { get; set; }

        public int ApplicationID { get; set; }

        public SelectionDecision Decision { get; set; }

        public string Notes { get; set; }

        public DateTime Date { get; set; }

        public Application Application { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}