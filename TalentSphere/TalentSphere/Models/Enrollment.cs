using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentSphere.Models
{
	public class Enrollment
	{
		[Key]
		public int EnrollmentID { get; set; }

		[Required]
		[ForeignKey("Training")]
		public int TrainingID { get; set; }

		[Required]
		[ForeignKey("Employee")]
		public int EmployeeID { get; set; }

		[Required]
		public DateOnly Date { get; set; }

		[Required]
		[StringLength(50)]
		public string status { get; set; }

		
	}
}
