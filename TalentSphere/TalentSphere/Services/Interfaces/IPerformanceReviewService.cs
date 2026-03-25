using TalentSphere.DTOs.PerformanceReview;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface IPerformanceReviewService
    {
        Task<PerformanceReviewDTO> CreateReviewAsync(CreatePerformanceReviewDTO dto);
        Task<PerformanceReviewDTO> GetByIdAsync(int id);
        Task<List<PerformanceReviewListDTO>> GetAllReviewsAsync(int? employeeId = null);
        Task<PerformanceReviewDTO> UpdateReviewAsync(int id, UpdatePerformanceReviewDTO dto);
    }
}
