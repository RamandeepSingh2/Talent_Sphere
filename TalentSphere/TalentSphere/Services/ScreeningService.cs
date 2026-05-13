using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class ScreeningService : IScreeningService
    {
        private readonly IScreeningRepository _repository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<ScreeningService> _logger;

        public ScreeningService(
            IScreeningRepository repository,
            IApplicationRepository applicationRepository,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<ScreeningService> logger)
        {
            _repository = repository;
            _applicationRepository = applicationRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ScreeningResponseDTO> CreateScreeningAsync(CreateScreeningDTO dto)
        {
            var application = await _applicationRepository.GetByIdWithDetailsAsync(dto.ApplicationID)
                ?? throw new KeyNotFoundException($"Application {dto.ApplicationID} not found.");

            if (application.Status == ApplicationStatus.Rejected || application.Status == ApplicationStatus.Accepted)
                throw new InvalidOperationException(
                    $"Cannot screen an application with status '{application.Status}'.");

            var screening = _mapper.Map<Screening>(dto);
            screening.Date = DateTime.UtcNow;
            screening.CreatedAt = DateTime.UtcNow;
            screening.IsDeleted = false;

            var added = await _repository.AddAsync(screening);

            // FIX: each result has its own clear outcome — no shared else if
            if (dto.Result == ScreeningResult.Pass)
            {
                // Passed — move to Reviewed so candidate appears in Interview "Ready to Interview" tab
                application.Status = ApplicationStatus.Reviewed;
                application.UpdatedAt = DateTime.UtcNow;
            }
            else if (dto.Result == ScreeningResult.Fail)
            {
                // Failed — reject immediately, pipeline ends here
                application.Status = ApplicationStatus.Rejected;
                application.UpdatedAt = DateTime.UtcNow;
            }
            // Pending — do NOT change status at all
            // Application stays Submitted and remains in Screening "Awaiting Screening" tab
            // Recruiter can come back and screen again later

            await _applicationRepository.SaveChangesAsync();
            await _repository.SaveChangesAsync();

            // Notify candidate
            try
            {
                var resultLabel = dto.Result switch
                {
                    ScreeningResult.Pass => "passed",
                    ScreeningResult.Fail => "did not pass",
                    _ => "is under review"
                };

                await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                {
                    UserID = application.CandidateID,
                    EntityID = added.ScreeningID,
                    Message = $"Your application for '{application.Job?.Title}' has been screened — result: {resultLabel}.",
                    Category = NotificationCategory.Recruitment
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Screening notification failed for ApplicationID {Id}", dto.ApplicationID);
            }

            return _mapper.Map<ScreeningResponseDTO>(added);
        }

        public async Task<ScreeningResponseDTO?> GetByIdAsync(int id)
        {
            var screening = await _repository.GetByIdAsync(id);
            return screening is null ? null : _mapper.Map<ScreeningResponseDTO>(screening);
        }

        public async Task<ScreeningResponseDTO?> GetByApplicationIdAsync(int applicationId)
        {
            var screening = await _repository.GetByApplicationIdAsync(applicationId);
            return screening is null ? null : _mapper.Map<ScreeningResponseDTO>(screening);
        }

        public async Task<IEnumerable<ScreeningResponseDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ScreeningResponseDTO>>(list);
        }

        public async Task<ScreeningResponseDTO?> UpdateScreeningAsync(int id, UpdateScreeningDTO dto)
        {
            var screening = await _repository.GetByIdAsync(id);
            if (screening is null) return null;

            var application = await _applicationRepository.GetByIdWithDetailsAsync(screening.ApplicationID);
            if (application != null)
            {
                // Block update if pipeline already moved past screening
                var lockedStatuses = new[]
                {
                    ApplicationStatus.Accepted,
                    ApplicationStatus.Interview,
                };

                if (lockedStatuses.Contains(application.Status))
                    throw new InvalidOperationException(
                        $"Cannot update screening: the application is '{application.Status}'. " +
                        "Screening results are locked once the candidate has moved to interview stage.");

                // Also block if already finally rejected at selection stage
                if (application.Status == ApplicationStatus.Rejected)
                {
                    // Only block if there is a Selection record (rejected at final stage)
                    // If rejected due to failed screening — allow edit
                    var existingScreening = await _repository.GetByIdAsync(id);
                    if (existingScreening?.Result == ScreeningResult.Fail &&
                        dto.Result != ScreeningResult.Fail)
                    {
                        // Recruiter changing from Fail to Pass — reopen the application
                        application.Status = ApplicationStatus.Reviewed;
                        application.UpdatedAt = DateTime.UtcNow;
                    }
                }

                // Sync application status with the new screening result
                if (dto.Result == ScreeningResult.Pass)
                {
                    application.Status = ApplicationStatus.Reviewed;
                    application.UpdatedAt = DateTime.UtcNow;
                }
                else if (dto.Result == ScreeningResult.Fail)
                {
                    application.Status = ApplicationStatus.Rejected;
                    application.UpdatedAt = DateTime.UtcNow;
                }
                else if (dto.Result == ScreeningResult.Pending)
                {
                    // Back to pending — reopen so it stays in Awaiting Screening tab
                    application.Status = ApplicationStatus.Submitted;
                    application.UpdatedAt = DateTime.UtcNow;
                }

                await _applicationRepository.SaveChangesAsync();
            }

            _mapper.Map(dto, screening);
            screening.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(screening);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ScreeningResponseDTO>(screening);
        }

        public async Task<bool> DeleteScreeningAsync(int id)
        {
            var screening = await _repository.GetByIdAsync(id);
            if (screening is null) return false;

            var application = await _applicationRepository.GetByIdWithDetailsAsync(screening.ApplicationID);
            if (application != null &&
                (application.Status == ApplicationStatus.Accepted ||
                 application.Status == ApplicationStatus.Interview))
                throw new InvalidOperationException(
                    "Cannot delete screening: the candidate is already in the interview stage or has been hired.");

            // If deleting a passed screening — revert application back to Submitted
            if (application != null && screening.Result == ScreeningResult.Pass)
            {
                application.Status = ApplicationStatus.Submitted;
                application.UpdatedAt = DateTime.UtcNow;
                await _applicationRepository.SaveChangesAsync();
            }

            screening.IsDeleted = true;
            screening.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(screening);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}