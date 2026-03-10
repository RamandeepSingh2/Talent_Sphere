using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentSphere.Models
{
	public class SuccessionPlan
	{
		[Key]
		public int SuccessionId { get; set; }

		[Required]
		[ForeignKey("Employee")]
		public int EmployeeId { get; set; }

		[Required]
		[StringLength(100)]
		public string Position { get; set; }
		[Required]
		[StringLength(255)]
		public string Timeline { get; set; }

		[StringLength(50)]
		public string status { get; set; }



	}
}
