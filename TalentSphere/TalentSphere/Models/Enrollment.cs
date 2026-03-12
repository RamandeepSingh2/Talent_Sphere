using System.ComponentModel.DataAnnotations.Schema;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
	public class Enrollment
	{
		public int EnrollmentID { get; set; }

		public int TrainingID { get; set; }
		
		public virtual Training Training { get; set; }

		public int EmployeeID { get; set; }

		public virtual Employee Employee { get; set; }

		public DateOnly Date { get; set; }

		public EnrollmentStatus status { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public bool IsDeleted { get; set; }

	}
}
