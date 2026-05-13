using TalentSphere.DTOs;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;
        private readonly ITrainingRepository _trainingRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(
            IEnrollmentRepository repository,
            ITrainingRepository trainingRepository,
            IEmployeeRepository employeeRepository,
            INotificationService notificationService,
            ILogger<EnrollmentService> logger)
        {
            _repository = repository;
            _trainingRepository = trainingRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<EnrollmentResponseDTO> CreateEnrollmentAsync(CreateEnrollmentDTO dto)
        {
            var training = await _trainingRepository.GetByIdAsync(dto.TrainingID)
                ?? throw new KeyNotFoundException($"Training {dto.TrainingID} not found.");

            if (training.status == TrainingStatus.Cancelled)
                throw new InvalidOperationException("Cannot enroll in a cancelled training.");

            // NEW: check max capacity
            if (training.MaxCapacity.HasValue)
            {
                var currentCount = await _repository.GetActiveCountByTrainingAsync(dto.TrainingID);
                if (currentCount >= training.MaxCapacity.Value)
                    throw new InvalidOperationException(
                        $"Training '{training.Title}' is full. Maximum capacity of {training.MaxCapacity} has been reached.");
            }

            // NEW: check duplicate enrollment
            var existing = await _repository.GetByEmployeeAndTrainingAsync(dto.EmployeeID, dto.TrainingID);
            if (existing != null)
                throw new InvalidOperationException(
                    $"This employee is already enrolled in '{training.Title}'.");

            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeID)
                ?? throw new KeyNotFoundException($"Employee {dto.EmployeeID} not found.");

            var now = DateTime.UtcNow;
            var trainingStarted = training.StartDate <= now;
            var enrollment = new Enrollment
            {
                TrainingID = dto.TrainingID,
                EmployeeID = dto.EmployeeID,
                Date = DateOnly.FromDateTime(now),
                DueDate = dto.DueDate,
                status = trainingStarted ? EnrollmentStatus.InProgress : EnrollmentStatus.Enrolled,
                StartedAt = trainingStarted ? now : null,
                CreatedAt = now,
                IsDeleted = false
            };

            var added = await _repository.AddAsync(enrollment);
            await _repository.SaveChangesAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                    {
                        UserID = employee.UserId,
                        EntityID = added.EnrollmentID,
                        Message = $"You have been enrolled in training: '{training.Title}'." +
                                  (dto.DueDate.HasValue ? $" Due by {dto.DueDate.Value:MMM d, yyyy}." : ""),
                        Category = NotificationCategory.Training
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Enrollment notification failed for EmployeeID {Id}", dto.EmployeeID);
                }
            });

            var withDetails = await _repository.GetByIdAsync(added.EnrollmentID);
            return MapToResponse(withDetails ?? added);
        }
        public async Task<EnrollmentResponseDTO?> GetByIdAsync(int id)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            return enrollment is null ? null : MapToResponse(enrollment);
        }

        public async Task<List<EnrollmentResponseDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToResponse).ToList();
        }

        public async Task<List<EnrollmentResponseDTO>> GetByEmployeeIdAsync(int employeeId)
        {
            var list = await _repository.GetByEmployeeIdAsync(employeeId);
            return list.Select(MapToResponse).ToList();
        }

        public async Task<EnrollmentResponseDTO?> StartEnrollmentAsync(int id)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            if (enrollment is null) return null;

            if (enrollment.status == EnrollmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot start a cancelled enrollment.");
            if (enrollment.status == EnrollmentStatus.Completed)
                throw new InvalidOperationException("Enrollment is already completed.");
            if (enrollment.status == EnrollmentStatus.InProgress)
                throw new InvalidOperationException("Enrollment is already in progress.");

            enrollment.status = EnrollmentStatus.InProgress;
            enrollment.StartedAt = DateTime.UtcNow;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(enrollment);
            await _repository.SaveChangesAsync();

            return MapToResponse(enrollment);
        }

        public async Task<EnrollmentResponseDTO?> CompleteEnrollmentAsync(int id, CompleteEnrollmentDTO dto)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            if (enrollment is null) return null;

            if (enrollment.status == EnrollmentStatus.Cancelled)
                throw new InvalidOperationException("Cannot complete a cancelled enrollment.");
            if (enrollment.status == EnrollmentStatus.Completed)
                throw new InvalidOperationException("Enrollment is already completed.");

            enrollment.status = EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTime.UtcNow;
            if (!enrollment.StartedAt.HasValue) enrollment.StartedAt = DateTime.UtcNow;
            if (dto.Score.HasValue) enrollment.Score = dto.Score;
            if (dto.Notes != null) enrollment.Notes = dto.Notes;
            if (dto.CertificateUrl != null) enrollment.CertificateUrl = dto.CertificateUrl;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(enrollment);
            await _repository.SaveChangesAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    var userId = enrollment.Employee?.UserId ?? 0;
                    if (userId == 0) return;
                    var scoreText = enrollment.Score.HasValue ? $" Score: {enrollment.Score}/100." : "";
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                    {
                        UserID = userId,
                        EntityID = id,
                        Message = $"Congratulations! You have completed training: '{enrollment.Training?.Title}'.{scoreText}",
                        Category = NotificationCategory.Training
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Completion notification failed for EnrollmentID {Id}", id);
                }
            });

            return MapToResponse(enrollment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var enrollment = await _repository.GetByIdAsync(id);
            if (enrollment is null) return false;

            enrollment.IsDeleted = true;
            enrollment.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(enrollment);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static EnrollmentResponseDTO MapToResponse(Enrollment e)
        {
            var isOverdue = e.status != EnrollmentStatus.Completed &&
                            e.status != EnrollmentStatus.Cancelled &&
                            e.DueDate.HasValue && e.DueDate.Value < DateTime.UtcNow;
            return new EnrollmentResponseDTO
            {
                EnrollmentID = e.EnrollmentID,
                EmployeeID = e.EmployeeID,
                EmployeeName = e.Employee?.Name,
                TrainingID = e.TrainingID,
                TrainingTitle = e.Training?.Title,
                TrainingType = e.Training?.TrainingType.ToString(),
                DeliveryMode = e.Training?.DeliveryMode.ToString(),
                TrainingLink = e.Training?.TrainingLink,
                ClassStartTime = e.Training?.ClassStartTime,
                ClassEndTime = e.Training?.ClassEndTime,
                TrainingStartDate = e.Training?.StartDate,
                TrainingEndDate = e.Training?.EndDate,
                Date = e.Date,
                DueDate = e.DueDate,
                StartedAt = e.StartedAt,
                CompletedAt = e.CompletedAt,
                Score = e.Score,
                Notes = e.Notes,
                CertificateUrl = e.CertificateUrl,
                Status = isOverdue ? "Overdue" : e.status.ToString(),
                IsOverdue = isOverdue,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            };
        }
    }
}
