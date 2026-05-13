using AutoMapper;
using TalentSphere.DTOs.Notification;
using TalentSphere.DTOs.PerformanceReview;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class PerformanceReviewService : IPerformanceReviewService
    {
        private readonly IPerformanceReviewRepository _repository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<PerformanceReviewService> _logger;

        public PerformanceReviewService(
            IPerformanceReviewRepository repository,
            IEmployeeRepository employeeRepository,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<PerformanceReviewService> logger)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PerformanceReviewDTO> CreateReviewAsync(CreatePerformanceReviewDTO dto, int managerId)
        {
            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeID)
                ?? throw new KeyNotFoundException($"Employee {dto.EmployeeID} not found.");

            var review = new PerformanceReview
            {
                EmployeeID = dto.EmployeeID,
                ManagerID = managerId,
                Score = dto.Rating,
                Comments = dto.Comments,
                Date = dto.ReviewDate,
                ReviewPeriod = dto.ReviewPeriod,
                AreasToImprove = dto.AreasToImprove,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var added = await _repository.AddAsync(review);
            await _repository.SaveChangesAsync();

            try
            {
                var periodText = !string.IsNullOrEmpty(dto.ReviewPeriod)
                    ? $" for {dto.ReviewPeriod}"
                    : string.Empty;

                await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                {
                    UserID = employee.UserId,
                    EntityID = added.ReviewID,
                    Message = $"A new performance review{periodText} has been submitted for you — rating: {dto.Rating}/5.",
                    Category = NotificationCategory.Performance
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Performance review notification failed for EmployeeID {Id}", dto.EmployeeID);
            }

            return MapToDTO(added);
        }

        public async Task<PerformanceReviewDTO?> UpdateReviewAsync(int id, UpdatePerformanceReviewDTO dto)
        {
            if (dto.Rating.HasValue && (dto.Rating.Value < 1 || dto.Rating.Value > 5))
                throw new ArgumentException("Rating must be between 1 and 5.");

            var review = await _repository.GetByIdAsync(id);
            if (review is null) return null;

            if (dto.Rating.HasValue) review.Score = dto.Rating.Value;
            if (dto.Comments != null) review.Comments = dto.Comments;
            if (dto.ReviewDate.HasValue) review.Date = dto.ReviewDate.Value;
            if (dto.ReviewPeriod != null) review.ReviewPeriod = dto.ReviewPeriod;
            if (dto.AreasToImprove != null) review.AreasToImprove = dto.AreasToImprove;
            review.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(review);
            await _repository.SaveChangesAsync();

            return MapToDTO(review);
        }

        public async Task<PerformanceReviewDTO?> GetByIdAsync(int id)
        {
            var review = await _repository.GetByIdAsync(id);
            return review is null ? null : MapToDTO(review);
        }

        public async Task<List<PerformanceReviewListDTO>> GetAllReviewsAsync(int? employeeId = null)
        {
            var reviews = await _repository.GetAllAsync(employeeId);
            return _mapper.Map<List<PerformanceReviewListDTO>>(reviews);
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _repository.GetByIdAsync(id);
            if (review is null) return false;

            review.IsDeleted = true;
            review.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(review);
            await _repository.SaveChangesAsync();
            return true;
        }

        private PerformanceReviewDTO MapToDTO(PerformanceReview r) =>
        _mapper.Map<PerformanceReviewDTO>(r);

    }
}