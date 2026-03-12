using System;
using System.ComponentModel.DataAnnotations;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
    public class Job
    {
        public int JobID { get; set; }

        public string Title { get; set; }

        public string Department { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public DateTime PostedDate { get; set; }

        public JobStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }


    }
}

