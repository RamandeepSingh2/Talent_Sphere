using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi;

namespace TalentSphere.Models
{
	public class Report
	{

		[Key]
		public int ReportID { get; set; }

		[Required]
		[StringLength(100)]
		public string Scope { get; set; }

		public string Metrics { get; set; }

		[Required]
		public DateOnly GenerateDate { get; set; }

	}
}
