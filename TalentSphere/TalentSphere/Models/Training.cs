using System.ComponentModel.DataAnnotations;
using TalentSphere.Enums;

namespace TalentSphere.Models
{
	public class Training
	{
		public int TrainingID { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string Duration { get; set; }

		public TrainingStatus status { get; set; }
        
		public DateTime CreatedAt { get; set; }
        
		public DateTime? UpdatedAt { get; set; }
		
		public bool IsDeleted { get; set; } 
	}
}