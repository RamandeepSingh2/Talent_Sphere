namespace TalentSphere.DTOs.PerformanceReview
{
    public class UpdatePerformanceReviewDTO
    {
        public int? Rating { get; set; }
        public string? Comments { get; set; }
        public DateTime? ReviewDate { get; set; }

        // NEW
        public string? ReviewPeriod { get; set; }
        public string? AreasToImprove { get; set; }
    }
}
