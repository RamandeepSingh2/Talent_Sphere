using System.ComponentModel.DataAnnotations;

namespace TalentSphere.Models
{
	public class Training
	{
		[Key]
		public int TrainingID { get; set; }

		[Required]
		[StringLength(255)]
		public string Title { get; set; }

		public string Description { get; set; }

		[Required]
		[StringLength(100)]
		public string Duration { get; set; }

		[Required]
		[StringLength(50)]
		public string status { get; set; }


	}
}